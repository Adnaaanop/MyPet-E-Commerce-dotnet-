using MyApp.DTOs.Cart;
using MyApp.Entities;

namespace MyApp.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetUserCartAsync(int userId);
        Task<IEnumerable<CartItem>> GetUserCartEntitiesAsync(int userId);
        Task<CartItemDto> AddItemAsync(int userId, AddCartItemRequest dto);
        Task<CartItemDto?> UpdateQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveItemAsync(int userId, int cartItemId);

        // ✅ New method
        Task ClearUserCartAsync(int userId);
    }
}
