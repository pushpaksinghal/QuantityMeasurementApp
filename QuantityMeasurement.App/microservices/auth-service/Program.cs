using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ──────────────────────────────────────────
builder.Logging.ClearProviders().AddConsole();

// ── Database ─────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found.");

builder.Services.AddDbContext<AuthDbContext>(o => o.UseSqlServer(connStr));

// ── Identity ─────────────────────────────────────────
builder.Services.AddIdentityCore<ApplicationUser>(o =>
{
    o.Password.RequireDigit = true;
    o.Password.RequiredLength = 6;
    o.Password.RequireUppercase = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AuthDbContext>()
.AddSignInManager();

// ── JWT Authentication ────────────────────────────────
var jwtKey      = builder.Configuration["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key missing.");
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultSignInScheme       = IdentityConstants.ExternalScheme;
})
.AddCookie(IdentityConstants.ExternalScheme)
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtIssuer,
        ValidAudience            = jwtAudience,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew                = TimeSpan.Zero
    };
})
.AddGoogle(o =>
{
    o.ClientId     = builder.Configuration["Authentication:Google:ClientId"]!;
    o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    o.CallbackPath = "/signin-google";
    o.SignInScheme  = IdentityConstants.ExternalScheme;
});

builder.Services.AddAuthorization();

// ── Application Services ─────────────────────────────
builder.Services.AddScoped<JwtTokenService>();

// ── Swagger ───────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => "Auth Service OK");

app.Run();
