using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using HistoryService.Data;
using HistoryService.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

// ── Database ──────────────────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection missing.");

builder.Services.AddDbContext<HistoryDbContext>(o => o.UseSqlServer(connStr));

// ── Redis Cache ───────────────────────────────────────────────────────────
var redisConn = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection missing.");

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration  = redisConn;
    o.InstanceName   = "HistoryService:";
});

// ── JWT ───────────────────────────────────────────────────────────────────
var jwtKey      = builder.Configuration["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key missing.");
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtIssuer,
        ValidAudience            = jwtAudience,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew                = TimeSpan.Zero
    });

builder.Services.AddAuthorization();

// ── Application services ──────────────────────────────────────────────────
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<HistoryCacheService>();
builder.Services.AddScoped<HistoryApplicationService>();

// ── Controllers + Swagger ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "History Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(o =>
    o.AddPolicy("AllowGateway", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => "History Service OK");

app.Run();
