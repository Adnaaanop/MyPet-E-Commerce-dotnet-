using MyApp.DTOs.Auth;
using MyApp.Entities;

namespace MyApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
