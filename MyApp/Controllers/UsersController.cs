using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Common;
using MyApp.DTOs.Users;
using MyApp.Enums;
using MyApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/users/me
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetMyProfile()
        {
            try
            {
                var userId = GetUserId();
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(ApiResponse<UserDto>.FailResponse("User not found.", 404));

                return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Profile fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.FailResponse("Failed to fetch profile", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/users/me
        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            try
            {
                var userId = GetUserId();
                var updatedUser = await _userService.UpdateUserAsync(userId, dto);

                if (updatedUser == null)
                    return NotFound(ApiResponse<UserDto>.FailResponse("User not found.", 404));

                return Ok(ApiResponse<UserDto>.SuccessResponse(updatedUser, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.FailResponse("Failed to update profile", 500, new List<string> { ex.Message }));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers(
       [FromQuery] UserRole? role,
       [FromQuery] UserStatus? status,
       [FromQuery] string? search,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10)
        {
            try
            {
                // Convert enum to string/boolean for filtering in service
                string? roleStr = role.HasValue ? (role == UserRole.Admin ? "Admin" : "User") : null;
                string? statusStr = status.HasValue ? (status == UserStatus.Active ? "Active" : "Blocked") : null;

                var users = await _userService.GetAllUsersAsync(roleStr, statusStr, search);

                // Pagination
                if (pageSize > 0)
                    users = users.Skip((page - 1) * pageSize).Take(pageSize);

                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users, "Users fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.FailResponse("Failed to fetch users", 500, new List<string> { ex.Message }));
            }
        }


        // PUT: api/users/{id}/block (Admin only)
        [HttpPut("{id}/block")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> BlockUser(int id)
        {
            try
            {
                var success = await _userService.BlockUserAsync(id);

                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("User not found.", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "User has been blocked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to block user", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/users/{id}/unblock (Admin only)
        [HttpPut("{id}/unblock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> UnblockUser(int id)
        {
            try
            {
                var success = await _userService.UnblockUserAsync(id);

                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("User not found.", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "User has been unblocked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to unblock user", 500, new List<string> { ex.Message }));
            }
        }

        // DELETE: api/users/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);

                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("User not found.", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to delete user", 500, new List<string> { ex.Message }));
            }
        }
    }
}
