using AutoMapper;
using MyApp.Data;
using MyApp.DTOs.Cart;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemDto>> GetUserCartAsync(int userId)
        {
            var items = await _cartRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CartItemDto>>(items);
        }

        public async Task<IEnumerable<CartItem>> GetUserCartEntitiesAsync(int userId)
        {
            return await _cartRepository.GetByUserIdAsync(userId);
        }

        // ✅ Updated: increment quantity if item exists
        public async Task<CartItemDto> AddItemAsync(int userId, AddCartItemRequest dto)
        {
            var existingItem = await _cartRepository.GetByUserAndProductAsync(userId, dto.ProductId, dto.PetId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _cartRepository.UpdateAsync(existingItem);
                await _cartRepository.SaveChangesAsync();
                return _mapper.Map<CartItemDto>(existingItem);
            }

            var newItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                PetId = dto.PetId,
                Quantity = dto.Quantity
            };

            await _cartRepository.AddAsync(newItem);
            await _cartRepository.SaveChangesAsync();

            return _mapper.Map<CartItemDto>(newItem);
        }

        public async Task<CartItemDto?> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _cartRepository.GetByIdAsync(cartItemId);
            if (item == null) return null;

            item.Quantity = quantity;
            await _cartRepository.UpdateAsync(item);
            await _cartRepository.SaveChangesAsync();

            return _mapper.Map<CartItemDto>(item);
        }

        public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
        {
            var item = await _cartRepository.GetByIdAsync(cartItemId);
            if (item == null || item.UserId != userId) return false;

            await _cartRepository.DeleteAsync(item);
            await _cartRepository.SaveChangesAsync();
            return true;
        }

        public async Task ClearUserCartAsync(int userId)
        {
            var items = await _cartRepository.GetByUserIdAsync(userId);
            await _cartRepository.DeleteRangeAsync(items);
            await _cartRepository.SaveChangesAsync();
        }

    }
}
