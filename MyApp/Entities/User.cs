namespace MyApp.Entities
{
    public class User
    {
        public int Id { get; set; }                         // numeric for EF identity
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;   // plain text NOT allowed
        public string Role { get; set; } = "User";          // "User" | "Admin"
        public bool IsActive { get; set; } = true;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
