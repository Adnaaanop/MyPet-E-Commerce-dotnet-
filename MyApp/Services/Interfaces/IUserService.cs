using MyApp.DTOs.Users;

namespace MyApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);

        // ✅ New
        Task<bool> BlockUserAsync(int id);
        Task<bool> UnblockUserAsync(int id);
    }
}
