namespace MyApp.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? PetId { get; set; }
        public string? ProductName { get; set; }
        public string? PetName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public double? Rating { get; set; } // Nullable for pets
        public string? Breed { get; set; } // For pets
        public int? Age { get; set; } // For pets
    }
}