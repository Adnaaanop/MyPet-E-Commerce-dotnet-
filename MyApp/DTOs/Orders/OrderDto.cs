namespace MyApp.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }            // ✅ Added
        public DateTime PlacedAt { get; set; }
        public string Status { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Total { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
