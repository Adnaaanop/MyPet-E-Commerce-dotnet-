using Microsoft.AspNetCore.Mvc;
using MyApp.Services.Interfaces;
using MyApp.DTOs.Auth;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest(new { message = "Email already exists" });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                // Check if user exists to distinguish cause
                var existingUser = await _authService.GetUserByEmailAsync(request.Email);
                if (existingUser != null && !existingUser.IsActive)
                {
                    return Forbid("Your account has been blocked. Please contact support.");
                }

                return Unauthorized(new { message = "Invalid email or password" });
            }

            // If token is null -> blocked user (extra safeguard)
            if (string.IsNullOrEmpty(result.Token))
                return Forbid("Your account has been blocked. Please contact support.");

            return Ok(result);
        }
    }
}
