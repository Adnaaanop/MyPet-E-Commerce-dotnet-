using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem?> GetByIdAsync(int id);
        Task<IEnumerable<CartItem>> GetByUserIdAsync(int userId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(CartItem cartItem);
        Task SaveChangesAsync();
    }
}
