using MyApp.Entities;

namespace MyApp.Repositories.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetAllAsync();
        Task AddAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(Pet pet);
        Task SaveChangesAsync();
    }
}
