using Microsoft.AspNetCore.Mvc;
using MyApp.Services.Interfaces;
using MyApp.DTOs.Auth;
using MyApp.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

                // Set cookies for newly registered user
                SetTokenCookies(result);

                return Ok(ApiResponse<AuthResponse>.SuccessResponse(null, "Registered successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponse>.FailResponse("Registration failed", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                if (result == null)
                    return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password", 401));

                if (string.IsNullOrEmpty(result.AccessToken))
                    return StatusCode(403, ApiResponse<object>.FailResponse("Your account has been blocked", 403));

                // ✅ Set cookies
                SetTokenCookies(result);

                // ✅ Return user info + tokens in response for Swagger/frontend
                var responseData = new
                {
                    id = result.Id,
                    name = result.Name,
                    role = result.Role,
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    expiresAt = result.ExpiresAt
                };

                return Ok(ApiResponse<object>.SuccessResponse(responseData, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Login failed", 500, new List<string> { ex.Message }));
            }
        }



        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<object>>> Refresh()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    return Unauthorized(ApiResponse<object>.FailResponse("No refresh token found", 401));

                var result = await _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = refreshToken });

                if (result == null)
                    return Unauthorized(ApiResponse<object>.FailResponse("Invalid or expired refresh token", 401));

                // Set new cookies
                SetTokenCookies(new AuthResponse
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresAt = result.ExpiresAt
                });

                return Ok(ApiResponse<object>.SuccessResponse(null, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to refresh token", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/auth/revoke
        [HttpPost("revoke")]
        public async Task<ActionResult<ApiResponse<object>>> Revoke()
        {
            try
            {
                if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                {
                    await _authService.RevokeRefreshTokenAsync(refreshToken);
                }

                // Delete cookies
                Response.Cookies.Delete("accessToken");
                Response.Cookies.Delete("refreshToken");

                return Ok(ApiResponse<object>.SuccessResponse(null, "Logged out successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to revoke token", 500, new List<string> { ex.Message }));
            }
        }

        // Helper method to set cookies
        // Helper method to set cookies
        private void SetTokenCookies(AuthResponse auth)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                // cannot be accessed by JS
                Secure = true,                  // must be true for HTTPS
                SameSite = SameSiteMode.None,   // allow cross-origin requests
                Expires = auth.ExpiresAt,
                Path = "/"
            };

            Response.Cookies.Append("accessToken", auth.AccessToken, cookieOptions);
            Response.Cookies.Append("refreshToken", auth.RefreshToken, cookieOptions);
        }

    }
}
