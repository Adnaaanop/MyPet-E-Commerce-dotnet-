using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetAllAsync();

        // ✅ NEW: Get all pets with optional filtering, searching, and sorting
        Task<IEnumerable<Pet>> GetAllFilteredAsync(string? category, string? search, string? sortOrder);

        Task AddAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(Pet pet);
        Task SaveChangesAsync();
    }
}
