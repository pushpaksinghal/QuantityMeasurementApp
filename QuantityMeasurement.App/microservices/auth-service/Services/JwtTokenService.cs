using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using AuthService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services;

public class JwtTokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    public async Task<(string Token, DateTime ExpiresAt)> CreateTokenAsync(ApplicationUser user)
    {
        var jwtKey      = _config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key missing.");
        var jwtIssuer   = _config["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer missing.");
        var jwtAudience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");
        int expiryMins  = int.TryParse(_config["Jwt:ExpiryMinutes"], out int m) ? m : 60;

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier,     user.Id),
            new(ClaimTypes.Email,              user.Email ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(expiryMins);

        var token = new JwtSecurityToken(jwtIssuer, jwtAudience, claims, expires: expiry, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }
}
