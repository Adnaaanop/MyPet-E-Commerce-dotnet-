using AutoMapper;
using MyApp.DTOs.Wishlist;
using MyApp.DTOs.Cart;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            ICartService cartService,
            IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _cartService = cartService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WishlistItemDto>> GetUserWishlistAsync(int userId)
        {
            var items = await _wishlistRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<WishlistItemDto>>(items);
        }

        public async Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest request)
        {
            var existingItem = await _wishlistRepository.GetByUserAndProductAsync(userId, request.ProductId, request.PetId);
            if (existingItem != null)
                return _mapper.Map<WishlistItemDto>(existingItem);

            var newItem = new WishlistItem
            {
                UserId = userId,
                ProductId = request.ProductId,
                PetId = request.PetId
            };

            await _wishlistRepository.AddAsync(newItem);
            await _wishlistRepository.SaveChangesAsync();

            var addedItem = await _wishlistRepository.GetByIdAsync(newItem.Id);
            return _mapper.Map<WishlistItemDto>(addedItem);
        }

        public async Task<bool> RemoveItemAsync(int userId, int wishlistItemId)
        {
            var item = await _wishlistRepository.GetByIdAsync(wishlistItemId);
            if (item == null || item.UserId != userId) return false;

            await _wishlistRepository.DeleteAsync(item);
            await _wishlistRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartItemDto>> MoveAllToCartAsync(int userId)
        {
            var wishlistItems = await _wishlistRepository.GetByUserIdAsync(userId);
            if (!wishlistItems.Any()) return Enumerable.Empty<CartItemDto>();

            foreach (var item in wishlistItems)
            {
                await _cartService.AddItemAsync(userId, new AddCartItemRequest
                {
                    ProductId = item.ProductId,
                    PetId = item.PetId,
                    Quantity = 1
                });

                await _wishlistRepository.DeleteAsync(item);
            }

            await _wishlistRepository.SaveChangesAsync();
            return await _cartService.GetUserCartAsync(userId);
        }

        public async Task<CartItemDto?> MoveItemToCartAsync(int userId, int wishlistItemId)
        {
            var item = await _wishlistRepository.GetByIdAsync(wishlistItemId);
            if (item == null || item.UserId != userId) return null;

            var cartItem = await _cartService.AddItemAsync(userId, new AddCartItemRequest
            {
                ProductId = item.ProductId,
                PetId = item.PetId,
                Quantity = 1
            });

            await _wishlistRepository.DeleteAsync(item);
            await _wishlistRepository.SaveChangesAsync();

            return cartItem;
        }
    }
}