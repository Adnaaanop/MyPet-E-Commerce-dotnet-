using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Orders;
using MyApp.Entities;
using MyApp.Services.Interfaces;
using System.Security.Claims;
//using MyApp.DTOs.Cart;

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
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId)
                return NotFound("Order not found or not authorized.");

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId();

            // ✅ Get entities instead of DTOs
            var cart = await _cartService.GetUserCartEntitiesAsync(userId);
            if (cart == null || !cart.Any())
                return BadRequest("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                Address = request.Address,
                Total = cart.Sum(c => c.Quantity *
                                     (c.Product?.Price ?? c.Pet?.Price ?? 0)),
                Items = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    PetId = c.PetId,
                    Quantity = c.Quantity,
                    Price = (c.Product?.Price ?? c.Pet?.Price ?? 0)
                }).ToList()
            };

            var createdOrder = await _orderService.PlaceOrderAsync(order);

            // Clear cart after placing order
            await _cartService.ClearUserCartAsync(userId);

            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            return Ok(orderDto);
        }
    }
}
