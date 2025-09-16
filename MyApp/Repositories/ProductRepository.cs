using MyApp.Data;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // ✅ NEW: Filtering, searching, and sorting
        public async Task<IEnumerable<Product>> GetAllFilteredAsync(string? category, string? search, string? sortOrder)
        {
            var query = _context.Products.AsQueryable();

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

            // Sorting
            if (!string.IsNullOrEmpty(sortOrder))
            {
                query = sortOrder.ToLower() switch
                {
                    "price-asc" => query.OrderBy(p => p.Price),
                    "price-desc" => query.OrderByDescending(p => p.Price),
                    "rating-asc" => query.OrderBy(p => p.Rating),
                    "rating-desc" => query.OrderByDescending(p => p.Rating),
                    _ => query.OrderBy(p => p.Name) // default sort by name
                };
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
