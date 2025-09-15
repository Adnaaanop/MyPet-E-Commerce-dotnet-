using System;

namespace MyApp.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }   // link to User
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        // convenience flags
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsExpired && !IsRevoked;

        // Navigation
        public User User { get; set; } = null!;
    }
}
