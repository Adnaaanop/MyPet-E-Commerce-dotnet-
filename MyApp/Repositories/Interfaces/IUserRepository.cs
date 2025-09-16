using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);

        // ✅ Updated to support filtering
        Task<IEnumerable<User>> GetAllAsync(
            string? role = null,
            string? status = null,
            string? search = null);

        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();
    }
}
