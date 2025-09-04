using MyApp.DTOs.Cart;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;

namespace MyApp.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<CartItemDto>> GetUserCartAsync(int userId)
        {
            var items = await _cartRepository.GetByUserIdAsync(userId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<CartItemDto> AddItemAsync(int userId, AddCartItemRequest dto)
        {
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                PetId = dto.PetId,
                Quantity = dto.Quantity
            };

            await _cartRepository.AddAsync(cartItem);
            await _cartRepository.SaveChangesAsync();

            return MapToDto(cartItem);
        }

        public async Task<CartItemDto?> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
            if (cartItem == null) return null;

            cartItem.Quantity = quantity;
            await _cartRepository.UpdateAsync(cartItem);
            await _cartRepository.SaveChangesAsync();

            return MapToDto(cartItem);
        }

        public async Task<bool> RemoveItemAsync(int cartItemId)
        {
            var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
            if (cartItem == null) return false;

            await _cartRepository.DeleteAsync(cartItem);
            await _cartRepository.SaveChangesAsync();
            return true;
        }

        private static CartItemDto MapToDto(CartItem item)
        {
            return new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                PetId = item.PetId,
                Quantity = item.Quantity,
                ProductName = item.Product?.Name,
                PetName = item.Pet?.Name
            };
        }
    }
}
