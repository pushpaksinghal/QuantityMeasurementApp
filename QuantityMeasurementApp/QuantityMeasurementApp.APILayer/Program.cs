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

    // FIX 1: Redis is optional — don't throw if it's empty
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
    builder.Services.AddDistributedMemoryCache();
    // FIX 1: Only register Redis if a connection string is actually provided
    if (!string.IsNullOrEmpty(redisConnection))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "QuantityApp:";
        });
    }

    builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
    builder.Services.AddSingleton<UnitAdapterFactory>();
    builder.Services.AddSingleton<QuantityValidationService>();

    builder.Services.AddScoped<IQuantityConversionService, QuantityConversionService>();
    builder.Services.AddScoped<IQuantityArithmeticService, QuantityArithmeticService>();
    builder.Services.AddScoped<IQuantityHistoryRepository, QuantityHistoryRepository>();
    builder.Services.AddScoped<IQuantityApplicationService, QuantityApplicationService>();
    builder.Services.AddScoped<RedisCacheService>();
    builder.Services.AddScoped<JwtTokenService>();

    builder.Services.AddCors(options =>
    {
        // FIX 2: Removed .AllowCredentials() — it cannot be combined with AllowAnyOrigin()
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
    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapGet("/ping", () => "API is working");

    app.MapGet("/minimal/history", async (IQuantityApplicationService service) =>
    {
        var data = await service.GetAllRecordsAsync();
        return Results.Ok(data);
    });

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var quantityDb = services.GetRequiredService<QuantityDbContext>();
        quantityDb.Database.Migrate();

        var authDb = services.GetRequiredService<AuthDbContext>();
        authDb.Database.Migrate();
    }

    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Urls.Add($"http://*:{port}");

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