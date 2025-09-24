using MyApp.DTOs.Orders;
using MyApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Services.Interfaces
{
    public interface IOrderService
    {
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

        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        Task<OrderDto?> CancelOrderAsync(int id, int userId);
    }
}