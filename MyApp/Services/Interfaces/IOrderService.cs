using MyApp.DTOs.Orders;
using MyApp.Entities;

namespace MyApp.Services.Interfaces
{
    public interface IOrderService
    {
        // ✅ Now uses enum + numeric sortId
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(
            OrderStatus? status = null,
            int? sortId = null,
            int page = 1,
            int pageSize = 10);

        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> PlaceOrderAsync(Order order);
        Task<OrderDto> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);

        // ✅ Admin/User actions now use enum
        Task<OrderDto?> CancelOrderAsync(int id, int userId);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
