using MyApp.DTOs.Cart;
using MyApp.DTOs.Wishlist;

namespace MyApp.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetUserWishlistAsync(int userId);
        Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest dto);
        Task<bool> RemoveItemAsync(int userId, int wishlistItemId);
        Task<IEnumerable<CartItemDto>> MoveAllToCartAsync(int userId);
        Task<CartItemDto?> MoveItemToCartAsync(int userId, int wishlistItemId); // ✅ New
    }
}
