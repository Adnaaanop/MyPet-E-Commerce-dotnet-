using MyApp.DTOs.Wishlist;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;

namespace MyApp.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IEnumerable<WishlistItemDto>> GetUserWishlistAsync(int userId)
        {
            var items = await _wishlistRepository.GetByUserIdAsync(userId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest dto)
        {
            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                PetId = dto.PetId
            };

            await _wishlistRepository.AddAsync(wishlistItem);
            await _wishlistRepository.SaveChangesAsync();

            return MapToDto(wishlistItem);
        }

        public async Task<bool> RemoveItemAsync(int wishlistItemId)
        {
            var wishlistItem = await _wishlistRepository.GetByIdAsync(wishlistItemId);
            if (wishlistItem == null) return false;

            await _wishlistRepository.DeleteAsync(wishlistItem);
            await _wishlistRepository.SaveChangesAsync();
            return true;
        }

        private static WishlistItemDto MapToDto(WishlistItem item)
        {
            return new WishlistItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                PetId = item.PetId,
                ProductName = item.Product?.Name,
                PetName = item.Pet?.Name
            };
        }
    }
}
