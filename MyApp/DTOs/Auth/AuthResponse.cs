namespace MyApp.DTOs.Auth
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        // Tokens
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;

        // Expiry info (optional but helpful for frontend)
        public DateTime ExpiresAt { get; set; }
    }
}
