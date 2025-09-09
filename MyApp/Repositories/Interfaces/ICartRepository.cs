using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem?> GetByIdAsync(int id);
        Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
        Task<CartItem?> GetByUserAndProductAsync(int userId, int? productId, int? petId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(CartItem cartItem);
        Task SaveChangesAsync();

        // ✅ New method
        Task DeleteRangeAsync(IEnumerable<CartItem> cartItems);
    }
}
