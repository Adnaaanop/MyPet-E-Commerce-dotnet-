using MyApp.Enums;

namespace MyApp.DTOs.Users
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public UserStatus? Status { get; set; }
    }
}
