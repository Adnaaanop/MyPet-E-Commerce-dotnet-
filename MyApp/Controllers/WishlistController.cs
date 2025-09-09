using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Wishlist;
using MyApp.Services.Interfaces;
using System.Security.Claims;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddWishlistItemRequest dto)
        {
            var userId = GetUserId();

            if (dto.ProductId == null && dto.PetId == null)
                return BadRequest("Either ProductId or PetId must be provided.");

            var item = await _wishlistService.AddItemAsync(userId, dto);
            return Ok(item);
        }

        [HttpDelete("{wishlistItemId}")]
        public async Task<IActionResult> RemoveItem(int wishlistItemId)
        {
            var userId = GetUserId();
            var success = await _wishlistService.RemoveItemAsync(userId, wishlistItemId);

            if (!success) return NotFound("Wishlist item not found.");
            return NoContent();
        }

        // ✅ Move all wishlist items to cart
        [HttpPost("move-to-cart")]
        public async Task<IActionResult> MoveAllToCart()
        {
            var userId = GetUserId();
            var updatedCart = await _wishlistService.MoveAllToCartAsync(userId);

            if (!updatedCart.Any())
                return BadRequest("Wishlist is empty.");

            return Ok(new
            {
                message = "All wishlist items moved to cart successfully.",
                cart = updatedCart
            });
        }

        // ✅ Move single wishlist item to cart
        [HttpPost("{wishlistItemId}/move-to-cart")]
        public async Task<IActionResult> MoveItemToCart(int wishlistItemId)
        {
            var userId = GetUserId();
            var cartItem = await _wishlistService.MoveItemToCartAsync(userId, wishlistItemId);

            if (cartItem == null)
                return NotFound("Wishlist item not found.");

            return Ok(new
            {
                message = "Wishlist item moved to cart successfully.",
                cartItem
            });
        }
    }
}
