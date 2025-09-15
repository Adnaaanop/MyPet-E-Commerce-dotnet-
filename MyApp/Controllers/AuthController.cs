using Microsoft.AspNetCore.Mvc;
using MyApp.Services.Interfaces;
using MyApp.DTOs.Auth;
using MyApp.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                if (result == null)
                    return BadRequest(ApiResponse<AuthResponse>.FailResponse("Email already exists", 400));

                return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Registered successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponse>.FailResponse("Registration failed", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                if (result == null)
                    return Unauthorized(ApiResponse<AuthResponse>.FailResponse("Invalid email or password", 401));

                if (string.IsNullOrEmpty(result.AccessToken))
                    return StatusCode(403, ApiResponse<AuthResponse>.FailResponse("Your account has been blocked. Please contact support.", 403));


                return Ok(ApiResponse<AuthResponse>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponse>.FailResponse("Login failed", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<RefreshResponse>>> Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);

                if (result == null)
                    return Unauthorized(ApiResponse<RefreshResponse>.FailResponse("Invalid or expired refresh token", 401));

                return Ok(ApiResponse<RefreshResponse>.SuccessResponse(result, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<RefreshResponse>.FailResponse("Failed to refresh token", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/auth/revoke
        [HttpPost("revoke")]
        public async Task<ActionResult<ApiResponse<object>>> Revoke([FromBody] RefreshRequest request)
        {
            try
            {
                await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Refresh token revoked successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to revoke refresh token", 500, new List<string> { ex.Message }));
            }
        }
    }
}
