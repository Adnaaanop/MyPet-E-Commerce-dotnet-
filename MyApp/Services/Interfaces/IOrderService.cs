using MyApp.DTOs.Orders;
using MyApp.Entities;

namespace MyApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> PlaceOrderAsync(Order order);
        Task<OrderDto> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
    }
}
