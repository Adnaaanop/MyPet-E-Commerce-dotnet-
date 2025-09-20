using MyApp.Entities;

namespace MyApp.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string Address { get; set; } = null!;

        // ✅ Optional, defaults to Placed if not provided
        public OrderStatus Status { get; set; } = OrderStatus.Placed;

        //public decimal Total { get; set; }

        // ✅ Add Items so you can send cart/order items when placing an order
        //public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
