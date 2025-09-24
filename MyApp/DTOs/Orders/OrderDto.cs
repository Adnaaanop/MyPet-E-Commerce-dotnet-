using MyApp.Entities;

namespace MyApp.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime PlacedAt { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public string PaymentId { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}