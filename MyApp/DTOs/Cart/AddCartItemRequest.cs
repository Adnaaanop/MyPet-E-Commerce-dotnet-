namespace MyApp.DTOs.Cart
{
    public class AddCartItemRequest
    {
        public int? ProductId { get; set; }
        public int? PetId { get; set; }
        public int Quantity { get; set; }
    }
}
