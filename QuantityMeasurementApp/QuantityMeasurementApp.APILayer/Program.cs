using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using QuantityMeasurementApp.APILayer.Filters;
using QuantityMeasurementApp.BusinessLayer.Factories;
using QuantityMeasurementApp.BusinessLayer.Interface;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.RepositoryLayer.ConnectionFactory;
using QuantityMeasurementApp.RepositoryLayer.Context;
using QuantityMeasurementApp.RepositoryLayer.Interfaces;
using QuantityMeasurementApp.RepositoryLayer.Records;
using QuantityMeasurementApp.RepositoryLayer.Utility;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // Add services to the container.
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<ActionLoggingFilter>();
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<GlobalExceptionFilter>();
    builder.Services.AddScoped<ActionLoggingFilter>();

    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection not found.");

    string redisConnection = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection not found.");

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
    .AddApiEndpoints();

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "QuantityApp:";

    });
    builder.Services.AddDbContext<QuantityDbContext>(options =>
        options.UseSqlServer(connectionString));
    // Register your app dependencies
    builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

    builder.Services.AddSingleton<UnitAdapterFactory>();
    builder.Services.AddSingleton<QuantityValidationService>();

    builder.Services.AddScoped<IQuantityConversionService, QuantityConversionService>();
    builder.Services.AddScoped<IQuantityArithmeticService, QuantityArithmeticService>();
    builder.Services.AddScoped<IQuantityHistoryRepository, QuantityHistoryRepository>();
    builder.Services.AddScoped<IQuantityApplicationService, QuantityApplicationService>();
    builder.Services.AddScoped<RedisCacheService>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("THIS_IS_A_DEMO_SECRET_KEY_123456"))
        };
    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHttpsRedirection();

    app.MapControllers();
    // Minimal APIs
    app.MapGet("/ping", () => "API is working");
    app.MapIdentityApi<ApplicationUser>();
    app.MapGet("/minimal/history", async (IQuantityApplicationService service) =>
    {
        var data = await service.GetAllRecordsAsync();
        return Results.Ok(data);
    });

    app.MapPost("/minimal/history", async (
        QuantityHistoryRecord record,
        IQuantityApplicationService service) =>
    {
        await service.AddRecordAsync(record);
        return Results.Ok("Record added");
    });

    app.Run();

}
catch(Exception ex)
{
    logger.Error(ex, "Application stopped because of an exception");
    throw;
}

finally
{
    LogManager.Shutdown();
}