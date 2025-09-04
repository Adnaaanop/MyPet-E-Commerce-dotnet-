using MyApp.DTOs.Wishlist;

namespace MyApp.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetUserWishlistAsync(int userId);
        Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest dto);
        Task<bool> RemoveItemAsync(int wishlistItemId);
    }
}
