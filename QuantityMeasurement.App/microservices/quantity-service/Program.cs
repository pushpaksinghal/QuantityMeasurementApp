using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuantityService.Adapters;
using QuantityService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole();

// ── JWT (validate tokens issued by Auth Service) ──────────────────────────
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

// ── Typed HTTP client → History Service ───────────────────────────────────
var historyBaseUrl = builder.Configuration["Services:HistoryService"]
    ?? throw new InvalidOperationException("Services:HistoryService URL missing.");

builder.Services.AddHttpClient<IHistoryClient, HistoryClient>(c =>
{
    c.BaseAddress = new Uri(historyBaseUrl);
    c.Timeout     = TimeSpan.FromSeconds(5);
});

// ── Domain services ───────────────────────────────────────────────────────
builder.Services.AddSingleton<UnitAdapterFactory>();
builder.Services.AddScoped<IConversionService, ConversionService>();
builder.Services.AddScoped<IArithmeticService, ArithmeticService>();
builder.Services.AddScoped<QuantityOrchestrator>();

// ── Controllers + Swagger ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Quantity Service", Version = "v1" });
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
app.MapGet("/health", () => "Quantity Service OK");

app.Run();
