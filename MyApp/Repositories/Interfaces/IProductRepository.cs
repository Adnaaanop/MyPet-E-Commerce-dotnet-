using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();

        // ✅ Updated: Get all products with optional filtering, searching, and sorting
        Task<IEnumerable<Product>> GetAllFilteredAsync(string? category, string? search, string? sortOrder);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
    }
}
