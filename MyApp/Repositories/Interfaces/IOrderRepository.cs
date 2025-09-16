using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // ✅ Enhanced GetAllAsync with optional filter, sort, and pagination
        Task<IEnumerable<Order>> GetAllAsync(
            string? status = null,
            string? sort = null,
            int page = 1,
            int pageSize = 10);

        Task<Order> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
