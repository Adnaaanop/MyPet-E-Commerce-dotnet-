using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Cart;
using MyApp.DTOs.Common;
using MyApp.Services.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CartItemDto>>>> GetUserCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetUserCartAsync(userId);
                return Ok(ApiResponse<IEnumerable<CartItemDto>>.SuccessResponse(cart, "Cart fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CartItemDto>>.FailResponse("Failed to fetch cart", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> AddItem([FromBody] AddCartItemRequest dto)
        {
            try
            {
                if (dto.ProductId == null && dto.PetId == null)
                    return BadRequest(ApiResponse<CartItemDto>.FailResponse("Either ProductId or PetId must be provided.", 400));

                var userId = GetUserId();
                var item = await _cartService.AddItemAsync(userId, dto);

                return CreatedAtAction(nameof(GetUserCart), new { }, ApiResponse<CartItemDto>.SuccessResponse(item, "Item added to cart successfully", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartItemDto>.FailResponse("Failed to add item to cart", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/cart/{cartItemId}
        [HttpPut("{cartItemId}")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> UpdateQuantity(int cartItemId, [FromBody] UpdateCartItemRequest dto)
        {
            try
            {
                var item = await _cartService.UpdateQuantityAsync(cartItemId, dto.Quantity);
                if (item == null)
                    return NotFound(ApiResponse<CartItemDto>.FailResponse("Cart item not found.", 404));

                return Ok(ApiResponse<CartItemDto>.SuccessResponse(item, "Cart item quantity updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartItemDto>.FailResponse("Failed to update cart item", 500, new List<string> { ex.Message }));
            }
        }

        // DELETE: api/cart/{cartItemId}
        [HttpDelete("{cartItemId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveItem(int cartItemId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _cartService.RemoveItemAsync(userId, cartItemId);

                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("Cart item not found.", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "Cart item removed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to remove cart item", 500, new List<string> { ex.Message }));
            }
        }

        // DELETE: api/cart/clear
        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<object>>> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearUserCartAsync(userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Cart cleared successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to clear cart", 500, new List<string> { ex.Message }));
            }
        }
    }
}
