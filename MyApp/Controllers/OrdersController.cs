using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Orders;
using MyApp.DTOs.Common;
using MyApp.Entities;
using MyApp.Services.Interfaces;
using System.Security.Claims;

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
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetMyOrders(
            [FromQuery] OrderStatus? status,
            [FromQuery] int? sortId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                var orders = await _orderService.GetAllOrdersAsync(status, sortId, page, pageSize);
                var userOrders = orders.Where(o => o.UserId == userId).ToList();

                var totalItems = userOrders.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var response = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Orders = userOrders
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Orders fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to fetch orders", 500, new List<string> { ex.Message }));
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

                return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order fetched successfully"));
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
                    Status = request.Status,
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

                return Ok(ApiResponse<OrderDto>.SuccessResponse(createdOrder, "Order placed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to place order", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/orders/{id}/status (Admin only)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] int newStatusId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrderStatus), newStatusId))
                    return BadRequest(ApiResponse<OrderDto>.FailResponse("Invalid status ID.", 400));

                var newStatus = (OrderStatus)newStatusId;

                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, newStatus);
                if (updatedOrder == null)
                    return NotFound(ApiResponse<OrderDto>.FailResponse("Order not found.", 404));

                return Ok(ApiResponse<OrderDto>.SuccessResponse(updatedOrder, "Order status updated successfully"));
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

                return Ok(ApiResponse<OrderDto>.SuccessResponse(cancelledOrder, "Order cancelled successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to cancel order", 500, new List<string> { ex.Message }));
            }
        }
    }
}
