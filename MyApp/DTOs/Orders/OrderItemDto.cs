namespace MyApp.DTOs.Orders
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int? PetId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
