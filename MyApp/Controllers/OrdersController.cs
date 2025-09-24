using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Common;
using MyApp.DTOs.Orders;
using MyApp.Entities;
using MyApp.Services.Interfaces;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

        public OrdersController(IOrderService orderService, ICartService cartService, IMapper mapper, IConfiguration configuration)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException("User ID not found in token.");

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
                var userOrders = orders?.Where(o => o.UserId == userId).ToList() ?? new List<OrderDto>();

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

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllOrdersForAdmin(
            [FromQuery] OrderStatus? status,
            [FromQuery] int? sortId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(status, sortId, page, pageSize);

                var totalItems = orders?.Count() ?? 0;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var response = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Orders = orders
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "All orders fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to fetch all orders", 500, new List<string> { ex.Message }));
            }
        }

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

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderDto>>> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetUserCartEntitiesAsync(userId) ?? new List<CartItem>();

                if (!cart.Any())
                    return BadRequest(ApiResponse<OrderDto>.FailResponse("Cart is empty.", 400));

                var order = new MyApp.Entities.Order
                {
                    UserId = userId,
                    Street = request.Street,
                    City = request.City,
                    Pincode = request.Pincode,
                    Status = request.Status,
                    Total = cart.Sum(c => c.Quantity * (c.Product?.Price ?? c.Pet?.Price ?? 0)),
                    Items = cart.Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        PetId = c.PetId,
                        Quantity = c.Quantity,
                        Price = c.Product?.Price ?? c.Pet?.Price ?? 0
                    }).ToList()
                };

                var createdOrder = await _orderService.PlaceOrderAsync(order);
                if (createdOrder != null)
                    await _cartService.ClearUserCartAsync(userId);

                return Ok(ApiResponse<OrderDto>.SuccessResponse(createdOrder ?? throw new InvalidOperationException("Order creation failed"), "Order placed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderDto>.FailResponse("Failed to place order", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPost("create-razorpay-order")]
        public async Task<ActionResult<ApiResponse<object>>> CreateRazorpayOrder([FromBody] CreateRazorpayOrderRequest request)
        {
            try
            {
                var userId = GetUserId();
                var total = request.Total;

                var client = new RazorpayClient(
                    _configuration["Razorpay:KeyId"],
                    _configuration["Razorpay:KeySecret"]);

                var razorpayOrder = new Dictionary<string, object>
                {
                    {"amount", total * 100}, // Amount in paise
                    {"currency", "INR"},
                    {"receipt", $"order_{userId}_{DateTime.UtcNow.Ticks}"}
                };

                var order = await Task.Run(() => client.Order.Create(razorpayOrder));
                var orderId = order["id"];

                var tempOrder = new MyApp.Entities.Order
                {
                    UserId = userId,
                    Total = total,
                    Status = OrderStatus.Pending,
                    Street = request.Street,
                    City = request.City,
                    Pincode = request.Pincode,
                    PlacedAt = DateTime.UtcNow
                };

                var savedOrder = await _orderService.PlaceOrderAsync(tempOrder);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    orderId = orderId,
                    amount = total * 100,
                    currency = "INR",
                    key = _configuration["Razorpay:KeyId"],
                    name = "PetPalace",
                    description = "Order Payment",
                    image = "https://your-logo-url.com/logo.png",
                    prefill = new { name = "Customer", email = "customer@example.com", contact = "9999999999" },
                    theme = new { color = "#f97316" },
                    handler = "/api/orders/razorpay-handler"
                }, "Razorpay order created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to create Razorpay order", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPost("razorpay-handler")]
        public async Task<IActionResult> RazorpayHandler([FromBody] RazorpayHandlerRequest request)
        {
            try
            {
                var client = new RazorpayClient(
                    _configuration["Razorpay:KeyId"],
                    _configuration["Razorpay:KeySecret"]);

                var attributes = new Dictionary<string, string>
                {
                    {"razorpay_order_id", request.RazorpayOrderId},
                    {"razorpay_payment_id", request.RazorpayPaymentId},
                    {"razorpay_signature", request.RazorpaySignature}
                };

                // Manual signature verification as Utility might not be available
                string secret = _configuration["Razorpay:KeySecret"];
                string toSign = $"{request.RazorpayOrderId}|{request.RazorpayPaymentId}";
                using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret)))
                {
                    var computedSignature = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toSign)));
                    var result = computedSignature == request.RazorpaySignature;
                    if (!result)
                    {
                        return BadRequest("Invalid payment signature");
                    }
                }

                var userId = GetUserId();
                var orders = await _orderService.GetAllOrdersAsync(OrderStatus.Pending);
                var order = orders.FirstOrDefault(o => o.UserId == userId);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                var dbOrder = await _orderService.GetOrderByIdAsync(order.Id);
                if (dbOrder == null) return NotFound("Order not found in database");

                var orderEntity = _mapper.Map<MyApp.Entities.Order>(dbOrder);
                orderEntity.Status = OrderStatus.Paid;
                orderEntity.PaymentId = request.RazorpayPaymentId;
                orderEntity.PaymentStatus = "paid";

                await _orderService.UpdateOrderAsync(orderEntity);
                await _cartService.ClearUserCartAsync(userId);

                var orderDto = _mapper.Map<OrderDto>(orderEntity);
                return Ok(new { success = true, message = "Payment successful", order = orderDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Payment verification failed" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromQuery] int newStatusId)
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

    public class CreateOrderRequest
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
    }

    public class CreateRazorpayOrderRequest
    {
        public decimal Total { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }

    public class RazorpayHandlerRequest
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }
}