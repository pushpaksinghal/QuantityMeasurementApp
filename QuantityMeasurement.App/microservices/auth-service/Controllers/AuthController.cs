using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Services;
using Shared.Contracts;
using System.Security.Claims;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtService,
        ILogger<AuthController> logger)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
        _jwtService   = jwtService;
        _logger       = logger;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        _logger.LogInformation("Register called for {Email}", request.Email);

        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest("User already exists.");

        var user   = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var (token, expiresAt) = await _jwtService.CreateTokenAsync(user);
        return Ok(new AuthResponseDto(token, user.Email!, expiresAt));
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        _logger.LogInformation("Login called for {Email}", request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid email or password.");

        var (token, expiresAt) = await _jwtService.CreateTokenAsync(user);
        return Ok(new AuthResponseDto(token, user.Email!, expiresAt));
    }

    // GET api/auth/google-login
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
    }

    // GET api/auth/google-response
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!auth.Succeeded) return Redirect("http://localhost:4200?error=google_auth_failed");

        var email = auth.Principal?.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email)) return Redirect("http://localhost:4200?error=email_not_received");

        var user = await _userManager.FindByEmailAsync(email)
                   ?? new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };

        if (user.Id == null)
        {
            var created = await _userManager.CreateAsync(user);
            if (!created.Succeeded) return Redirect("http://localhost:4200?error=user_creation_failed");
        }

        var (token, _) = await _jwtService.CreateTokenAsync(user);
        return Redirect($"http://localhost:4200/google-callback?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}");
    }
}
