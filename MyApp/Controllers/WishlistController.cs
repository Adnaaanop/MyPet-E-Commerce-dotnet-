using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Cart;
using MyApp.DTOs.Common;
using MyApp.DTOs.Wishlist;
using MyApp.Services.Interfaces;
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
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/wishlist
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<WishlistItemDto>>>> GetWishlist()
        {
            try
            {
                var userId = GetUserId();
                var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
                return Ok(ApiResponse<IEnumerable<WishlistItemDto>>.SuccessResponse(wishlist, "Wishlist fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<WishlistItemDto>>.FailResponse("Failed to fetch wishlist", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/wishlist
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WishlistItemDto>>> AddItem([FromBody] AddWishlistItemRequest dto)
        {
            try
            {
                var userId = GetUserId();

                if (dto.ProductId == null && dto.PetId == null)
                    return BadRequest(ApiResponse<WishlistItemDto>.FailResponse("Either ProductId or PetId must be provided.", 400));

                var item = await _wishlistService.AddItemAsync(userId, dto);
                return Ok(ApiResponse<WishlistItemDto>.SuccessResponse(item, "Item added to wishlist successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<WishlistItemDto>.FailResponse("Failed to add item to wishlist", 500, new List<string> { ex.Message }));
            }
        }

        // DELETE: api/wishlist/{wishlistItemId}
        [HttpDelete("{wishlistItemId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveItem(int wishlistItemId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _wishlistService.RemoveItemAsync(userId, wishlistItemId);

                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("Wishlist item not found.", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "Wishlist item removed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to remove wishlist item", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/wishlist/move-to-cart
        [HttpPost("move-to-cart")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CartItemDto>>>> MoveAllToCart()
        {
            try
            {
                var userId = GetUserId();
                var updatedCart = await _wishlistService.MoveAllToCartAsync(userId);

                if (!updatedCart.Any())
                    return BadRequest(ApiResponse<IEnumerable<CartItemDto>>.FailResponse("Wishlist is empty.", 400));

                return Ok(ApiResponse<IEnumerable<CartItemDto>>.SuccessResponse(updatedCart, "All wishlist items moved to cart successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CartItemDto>>.FailResponse("Failed to move wishlist items to cart", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/wishlist/{wishlistItemId}/move-to-cart
        [HttpPost("{wishlistItemId}/move-to-cart")]
        public async Task<ActionResult<ApiResponse<CartItemDto>>> MoveItemToCart(int wishlistItemId)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _wishlistService.MoveItemToCartAsync(userId, wishlistItemId);

                if (cartItem == null)
                    return NotFound(ApiResponse<CartItemDto>.FailResponse("Wishlist item not found.", 404));

                return Ok(ApiResponse<CartItemDto>.SuccessResponse(cartItem, "Wishlist item moved to cart successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartItemDto>.FailResponse("Failed to move wishlist item to cart", 500, new List<string> { ex.Message }));
            }
        }
    }
}
