using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.BusinessLayer.Services;
using QuantityMeasurementApp.ModelLayer.DTOs;
using QuantityMeasurementApp.RepositoryLayer.Context;
using System.Security.Claims;

namespace QuantityMeasurementApp.APILayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("Register called for email {Email}", request.Email);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. Email already exists: {Email}", request.Email);
                return BadRequest("User already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogWarning("Registration failed for email {Email}", request.Email);
                return BadRequest(errors);
            }

            var tokenResult = await _jwtTokenService.CreateTokenAsync(user);

            _logger.LogInformation("Registration successful for email {Email}", request.Email);

            return Ok(new AuthResponseDto
            {
                Token = tokenResult.Token,
                Email = user.Email ?? string.Empty,
                ExpiresAt = tokenResult.ExpiresAt
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Login called for email {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found for email {Email}", request.Email);
                return Unauthorized("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed. Invalid password for email {Email}", request.Email);
                return Unauthorized("Invalid email or password.");
            }

            var tokenResult = await _jwtTokenService.CreateTokenAsync(user);

            _logger.LogInformation("Login successful for email {Email}", request.Email);

            return Ok(new AuthResponseDto
            {
                Token = tokenResult.Token,
                Email = user.Email ?? string.Empty,
                ExpiresAt = tokenResult.ExpiresAt
            });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!authenticateResult.Succeeded)
            {
                return Redirect("http://127.0.0.1:5500/index.html?error=google_auth_failed");
            }

            var email = authenticateResult.Principal?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return Redirect("http://127.0.0.1:5500/index.html?error=email_not_received");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Redirect("http://127.0.0.1:5500/index.html?error=user_creation_failed");
                }
            }

            var tokenResult = await _jwtTokenService.CreateTokenAsync(user);

            var frontendRedirect =
                $"http://127.0.0.1:5500/google-callback.html?token={Uri.EscapeDataString(tokenResult.Token)}" +
                $"&email={Uri.EscapeDataString(user.Email ?? string.Empty)}";

            return Redirect(frontendRedirect);
        }
    }
}