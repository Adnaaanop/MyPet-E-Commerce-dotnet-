namespace MyApp.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? PetId { get; set; }

        // Extra info for frontend display
        public string? ProductName { get; set; }
        public string? PetName { get; set; }
    }
}
