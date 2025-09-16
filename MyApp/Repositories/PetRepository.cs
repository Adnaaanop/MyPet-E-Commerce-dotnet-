using MyApp.Data;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        // ✅ NEW: Filtering, searching, and sorting logic
        public async Task<IEnumerable<Pet>> GetAllFilteredAsync(string? category, string? search, string? sortOrder)
        {
            var query = _context.Pets.AsQueryable();

            // Filter by category
            if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
            {
                query = query.Where(p => p.Category.ToLower() == category.ToLower());
            }

            // Search by name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            // Sort by price
            if (!string.IsNullOrEmpty(sortOrder))
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(p => p.Price),
                    "desc" => query.OrderByDescending(p => p.Price),
                    _ => query
                };
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
        }

        public Task UpdateAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Pet pet)
        {
            _context.Pets.Remove(pet);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
