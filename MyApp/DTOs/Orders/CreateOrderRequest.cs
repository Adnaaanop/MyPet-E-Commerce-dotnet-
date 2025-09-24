using MyApp.Entities;

namespace MyApp.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
    }
}