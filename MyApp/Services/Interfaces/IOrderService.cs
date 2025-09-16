using MyApp.DTOs.Orders;
using MyApp.Entities;

namespace MyApp.Services.Interfaces
{
    public interface IOrderService
    {
        // ✅ Enhanced GetAllOrdersAsync with filter, sort, and pagination
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(
            string? status = null,
            string? sort = null,
            int page = 1,
            int pageSize = 10);

        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> PlaceOrderAsync(Order order);
        Task<OrderDto> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);

        // ✅ Admin/User actions
        Task<OrderDto?> CancelOrderAsync(int id, int userId);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
