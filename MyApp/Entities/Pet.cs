namespace MyApp.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public int Age { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string Category { get; set; } = "Pet";
        public int Stock { get; set; } = 0;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
