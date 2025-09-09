using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Cart;
using MyApp.Services.Interfaces;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetUserCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest dto)
        {
            if (dto.ProductId == null && dto.PetId == null)
                return BadRequest("Either ProductId or PetId must be provided.");

            var userId = GetUserId();
            var item = await _cartService.AddItemAsync(userId, dto);
            return CreatedAtAction(nameof(GetUserCart), new { }, item);
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateCartItemRequest dto)
        {
            var item = await _cartService.UpdateQuantityAsync(cartItemId, dto.Quantity);
            if (item == null) return NotFound("Cart item not found.");
            return Ok(item);
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userId = GetUserId();
            var success = await _cartService.RemoveItemAsync(userId, cartItemId);

            if (!success) return NotFound("Cart item not found.");
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearUserCartAsync(userId);
            return NoContent();
        }
    }
}
