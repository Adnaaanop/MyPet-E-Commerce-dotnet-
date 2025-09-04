using MyApp.DTOs.Cart;

namespace MyApp.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetUserCartAsync(int userId);
        Task<CartItemDto> AddItemAsync(int userId, AddCartItemRequest dto);
        Task<CartItemDto?> UpdateQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveItemAsync(int cartItemId);
    }
}
