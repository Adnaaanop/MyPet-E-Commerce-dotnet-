namespace MyApp.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? PetId { get; set; }
        public int Quantity { get; set; }

        // Extra info for displaying in frontend
        public string? ProductName { get; set; }
        public string? PetName { get; set; }

        // New fields
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
    }
}