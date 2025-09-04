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
    }
}
