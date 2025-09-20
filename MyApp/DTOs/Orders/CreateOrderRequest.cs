using MyApp.Entities;

namespace MyApp.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string Address { get; set; } = string.Empty;

        // ✅ Frontend sends numeric ID
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
    }
}
