using MyApp.Data;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace MyApp.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WishlistItem?> GetByIdAsync(int id)
        {
            return await _context.WishlistItems
                .Include(w => w.Product)
                .Include(w => w.Pet)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId)
        {
            return await _context.WishlistItems
                .Include(w => w.Product)
                .Include(w => w.Pet)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<WishlistItem?> GetByUserAndProductAsync(int userId, int? productId, int? petId)
        {
            return await _context.WishlistItems
                .FirstOrDefaultAsync(w =>
                    w.UserId == userId &&
                    w.ProductId == productId &&
                    w.PetId == petId);
        }

        public async Task AddAsync(WishlistItem wishlistItem)
        {
            await _context.WishlistItems.AddAsync(wishlistItem);
        }

        public Task DeleteAsync(WishlistItem wishlistItem)
        {
            _context.WishlistItems.Remove(wishlistItem);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
