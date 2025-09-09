using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<WishlistItem?> GetByIdAsync(int id);
        Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);
        Task<WishlistItem?> GetByUserAndProductAsync(int userId, int? productId, int? petId);
        Task AddAsync(WishlistItem wishlistItem);
        Task DeleteAsync(WishlistItem wishlistItem);
        Task SaveChangesAsync();
    }
}
