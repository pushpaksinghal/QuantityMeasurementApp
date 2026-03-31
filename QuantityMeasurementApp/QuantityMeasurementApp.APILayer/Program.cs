using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using QuantityMeasurementApp.APILayer.Filters;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.RepositoryLayer.ConnectionFactory;
using QuantityMeasurementApp.RepositoryLayer.Context;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using QuantityMeasurementApp.RepositoryLayer.Utility;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddScoped<GlobalExceptionFilter>();
    builder.Services.AddScoped<ActionLoggingFilter>();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<ActionLoggingFilter>();
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "QuantityMeasurementApp API", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT token"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection not found.");

    // Log connection string (without password for security)
    var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
    logger.Info($"Database connection configured for host: {connectionStringBuilder.Host}, database: {connectionStringBuilder.Database}");

    // Redis configuration
    string? redisConnection = builder.Configuration.GetConnectionString("Redis");

    string jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Jwt:Key not found.");

    string jwtIssuer = builder.Configuration["Jwt:Issuer"]
        ?? throw new InvalidOperationException("Jwt:Issuer not found.");

    string jwtAudience = builder.Configuration["Jwt:Audience"]
        ?? throw new InvalidOperationException("Jwt:Audience not found.");

    builder.Services.AddDbContext<QuantityDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddSignInManager();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie(IdentityConstants.ExternalScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });

    builder.Services.AddAuthorization();

    // ✅ FIXED: Configure caching with fallback
    if (!string.IsNullOrEmpty(redisConnection))
    {
        // Use Redis cache
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "QuantityApp:";
        });
        logger.Info("✅ Redis cache configured with Upstash");
    }
    else
    {
        // Fallback to in-memory cache when Redis is not available
        builder.Services.AddDistributedMemoryCache();
        logger.Warn("⚠️ Redis not configured - using in-memory cache fallback");
    }

    // ✅ Register RedisCacheService - it will work with either Redis or in-memory cache
    builder.Services.AddScoped<RedisCacheService>();

    builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
    builder.Services.AddSingleton<UnitAdapterFactory>();
    builder.Services.AddSingleton<QuantityValidationService>();

    builder.Services.AddScoped<IQuantityConversionService, QuantityConversionService>();
    builder.Services.AddScoped<IQuantityArithmeticService, QuantityArithmeticService>();
    builder.Services.AddScoped<IQuantityHistoryRepository, QuantityHistoryRepository>();
    builder.Services.AddScoped<IQuantityApplicationService, QuantityApplicationService>();
    builder.Services.AddScoped<JwtTokenService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Basic health check endpoint
    app.MapGet("/ping", () => "API is working");

    // Enhanced health check endpoint
    app.MapGet("/health", () => new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow,
        environment = builder.Environment.EnvironmentName
    });

    // Database health check endpoint
    app.MapGet("/health/database", async (QuantityDbContext quantityDb, AuthDbContext authDb) =>
    {
        var results = new Dictionary<string, object>();
        
        try
        {
            // Check QuantityDbContext
            var quantityCanConnect = await quantityDb.Database.CanConnectAsync();
            var quantityPendingMigrations = (await quantityDb.Database.GetPendingMigrationsAsync()).Count();
            var quantityAppliedMigrations = (await quantityDb.Database.GetAppliedMigrationsAsync()).Count();
            
            results["QuantityDbContext"] = new
            {
                connected = quantityCanConnect,
                pendingMigrations = quantityPendingMigrations,
                appliedMigrations = quantityAppliedMigrations,
                provider = quantityDb.Database.ProviderName
            };
            
            // Check AuthDbContext
            var authCanConnect = await authDb.Database.CanConnectAsync();
            var authPendingMigrations = (await authDb.Database.GetPendingMigrationsAsync()).Count();
            var authAppliedMigrations = (await authDb.Database.GetAppliedMigrationsAsync()).Count();
            
            results["AuthDbContext"] = new
            {
                connected = authCanConnect,
                pendingMigrations = authPendingMigrations,
                appliedMigrations = authAppliedMigrations,
                provider = authDb.Database.ProviderName
            };
            
            results["overall"] = quantityCanConnect && authCanConnect ? "Healthy" : "Degraded";
            
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            results["error"] = ex.Message;
            results["overall"] = "Unhealthy";
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }
    });

    // ✅ Add Redis health check endpoint
    app.MapGet("/health/redis", async (RedisCacheService redisCache) =>
    {
        try
        {
            await redisCache.SetAsync("health_check", "OK", TimeSpan.FromMinutes(1));
            var result = await redisCache.GetAsync<string>("health_check");
            
            return Results.Ok(new 
            { 
                status = result == "OK" ? "Connected" : "Failed",
                cacheType = redisConnection != null ? "Redis (Upstash)" : "In-Memory (Fallback)",
                configured = redisConnection != null,
                testResult = result
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Redis health check failed: {ex.Message}");
        }
    });

    app.MapGet("/minimal/history", async (IQuantityApplicationService service) =>
    {
        var data = await service.GetAllRecordsAsync();
        return Results.Ok(data);
    });

    // Apply migrations with detailed logging
    logger.Info("Starting database migration process...");
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        
        try
        {
            // Migrate QuantityDbContext
            var quantityDb = services.GetRequiredService<QuantityDbContext>();
            logger.Info("Checking QuantityDbContext for pending migrations...");
            
            var quantityPendingMigrations = await quantityDb.Database.GetPendingMigrationsAsync();
            var quantityPendingList = quantityPendingMigrations.ToList();
            
            if (quantityPendingList.Any())
            {
                logger.Info($"Found {quantityPendingList.Count} pending migrations for QuantityDbContext:");
                foreach (var migration in quantityPendingList)
                {
                    logger.Info($"  - {migration}");
                }
                
                logger.Info("Applying QuantityDbContext migrations...");
                await quantityDb.Database.MigrateAsync();
                logger.Info("✅ QuantityDbContext migrations completed successfully");
            }
            else
            {
                logger.Info("✅ No pending migrations for QuantityDbContext");
            }
            
            // Verify tables exist by checking if we can query
            try
            {
                var canConnect = await quantityDb.Database.CanConnectAsync();
                logger.Info($"QuantityDbContext connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "QuantityDbContext connection test failed");
            }
            
            // Migrate AuthDbContext
            var authDb = services.GetRequiredService<AuthDbContext>();
            logger.Info("Checking AuthDbContext for pending migrations...");
            
            var authPendingMigrations = await authDb.Database.GetPendingMigrationsAsync();
            var authPendingList = authPendingMigrations.ToList();
            
            if (authPendingList.Any())
            {
                logger.Info($"Found {authPendingList.Count} pending migrations for AuthDbContext:");
                foreach (var migration in authPendingList)
                {
                    logger.Info($"  - {migration}");
                }
                
                logger.Info("Applying AuthDbContext migrations...");
                await authDb.Database.MigrateAsync();
                logger.Info("✅ AuthDbContext migrations completed successfully");
            }
            else
            {
                logger.Info("✅ No pending migrations for AuthDbContext");
            }
            
            // Verify AuthDbContext connection
            try
            {
                var canConnect = await authDb.Database.CanConnectAsync();
                logger.Info($"AuthDbContext connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AuthDbContext connection test failed");
            }
            
            logger.Info("Database migration process completed");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "❌ Error applying database migrations");
            throw;
        }
    }

    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Urls.Add($"http://*:{port}");
    
    logger.Info($"Application starting on port {port}");
    logger.Info($"Health check available at /health and /health/database");
    logger.Info($"Redis health check available at /health/redis");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped because of an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}