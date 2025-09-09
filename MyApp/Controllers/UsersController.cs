using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Users;
using MyApp.Services.Interfaces;
using System.Security.Claims;

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

        // 🔹 GET: api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        // 🔹 PUT: api/users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto dto)
        {
            var userId = GetUserId();
            var updatedUser = await _userService.UpdateUserAsync(userId, dto);
            return Ok(updatedUser);
        }

        // 🔹 GET: api/users (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // 🔹 PUT: api/users/{id}/block (Admin only)
        [HttpPut("{id}/block")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var success = await _userService.BlockUserAsync(id);
            if (!success) return NotFound("User not found.");
            return Ok("User has been blocked.");
        }

        // 🔹 PUT: api/users/{id}/unblock (Admin only)
        [HttpPut("{id}/unblock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var success = await _userService.UnblockUserAsync(id);
            if (!success) return NotFound("User not found.");
            return Ok("User has been unblocked.");
        }

        // 🔹 DELETE: api/users/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound("User not found.");
            return Ok("User deleted.");
        }
    }
}
