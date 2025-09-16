using MyApp.DTOs.Users;

namespace MyApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);

        // ✅ Filtering support
        Task<IEnumerable<UserDto>> GetAllUsersAsync(
            string? role = null,
            string? status = null,
            string? search = null);

        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> BlockUserAsync(int id);
        Task<bool> UnblockUserAsync(int id);
    }
}
