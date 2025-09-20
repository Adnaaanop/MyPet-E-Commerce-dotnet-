using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // ✅ Now uses enum + numeric sortId instead of strings
        Task<IEnumerable<Order>> GetAllAsync(
            OrderStatus? status = null,
            int? sortId = null,
            int page = 1,
            int pageSize = 10);

        Task<Order> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
