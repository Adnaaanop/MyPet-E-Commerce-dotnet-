using AutoMapper;
using MyApp.DTOs.Orders;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(
            OrderStatus? status = null,
            int? sortId = null,
            int page = 1,
            int pageSize = 10)
        {
            var orders = await _orderRepository.GetAllAsync(status, sortId, page, pageSize);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> PlaceOrderAsync(Order order)
        {
            var createdOrder = await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
            var updated = await _orderRepository.GetByIdAsync(order.Id);
            return _mapper.Map<OrderDto>(updated);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            await _orderRepository.DeleteAsync(id);
            return true;
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> CancelOrderAsync(int id, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || order.UserId != userId)
                return null;

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
                return null;

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDto>(order);
        }
    }
}