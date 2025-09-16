using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Orders;
using MyApp.DTOs.Common;
using MyApp.Entities;
using MyApp.Services.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, ICartService cartService, IMapper mapper)
        {
            _orderService = orderService;
            _cartService = cartService;
            _mapper = mapper;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/orders
        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetMyOrders(
            [FromQuery] string? status,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();

                // Call service with filter, sort, and pagination
                var orders = await _orderService.GetAllOrdersAsync(status, sort, page, pageSize);

                // Only return orders of this user
                orders = orders.Where(o => o.UserId == userId);

                var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

                // Pagination info
                var totalItems = orderDtos.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var response = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Orders = orderDtos
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Orders fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.FailResponse("Failed to fetch orders", 500, new List<string> { ex.Message }));
            }
        }


        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(int id)
        {
            try
            {
                var userId = GetUserId();
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null || order.UserId != userId)
                    return NotFound(ApiResponse<OrderDto>.FailResponse("Order not found or not authorized.", 404));

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to fetch order", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetUserCartEntitiesAsync(userId);

                if (cart == null || !cart.Any())
                    return BadRequest(ApiResponse<OrderDto>.FailResponse("Cart is empty.", 400));

                var order = new Order
                {
                    UserId = userId,
                    Address = request.Address,
                    Total = cart.Sum(c => c.Quantity * (c.Product?.Price ?? c.Pet?.Price ?? 0)),
                    Items = cart.Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        PetId = c.PetId,
                        Quantity = c.Quantity,
                        Price = (c.Product?.Price ?? c.Pet?.Price ?? 0)
                    }).ToList()
                };

                var createdOrder = await _orderService.PlaceOrderAsync(order);

                await _cartService.ClearUserCartAsync(userId);

                var orderDto = _mapper.Map<OrderDto>(createdOrder);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order placed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to place order", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/orders/{id}/status (Admin only)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                var validStatuses = new[] { "Placed", "Shipped", "Delivered", "Cancelled" };
                if (!validStatuses.Contains(newStatus))
                    return BadRequest(ApiResponse<OrderDto>.FailResponse("Invalid status.", 400));

                var updated = await _orderService.UpdateOrderStatusAsync(id, newStatus);
                if (updated == null)
                    return NotFound(ApiResponse<OrderDto>.FailResponse("Order not found.", 404));

                var updatedDto = _mapper.Map<OrderDto>(updated);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(updatedDto, "Order status updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to update order status", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/orders/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CancelOrder(int id)
        {
            try
            {
                var userId = GetUserId();
                var cancelledOrder = await _orderService.CancelOrderAsync(id, userId);

                if (cancelledOrder == null)
                    return BadRequest(ApiResponse<OrderDto>.FailResponse("Order cannot be cancelled.", 400));

                var cancelledDto = _mapper.Map<OrderDto>(cancelledOrder);
                return Ok(ApiResponse<OrderDto>.SuccessResponse(cancelledDto, "Order cancelled successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to cancel order", 500, new List<string> { ex.Message }));
            }
        }
    }
}
