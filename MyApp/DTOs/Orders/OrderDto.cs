using MyApp.Entities;

namespace MyApp.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime PlacedAt { get; set; }

        // ✅ Changed from enum to string for API output
        public string Status { get; set; } = null!;

        public string Address { get; set; } = null!;
        public decimal Total { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
