using MyApp.Data;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // ✅ Filtering logic
        public async Task<IEnumerable<User>> GetAllAsync(string? role = null, string? status = null, string? search = null)
        {
            var query = _context.Users.AsQueryable();

            // Filter by Role
            if (!string.IsNullOrEmpty(role) && role != "All")
                query = query.Where(u => u.Role == role);

            // Filter by Status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                if (status == "Active")
                    query = query.Where(u => u.IsActive);
                else if (status == "Blocked")
                    query = query.Where(u => !u.IsActive);
            }

            // Search by name or email
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));

            return await query.ToListAsync();
        }

        public async Task AddAsync(User user) =>
            await _context.Users.AddAsync(user);

        public async Task UpdateAsync(User user) =>
            _context.Users.Update(user);

        public async Task DeleteAsync(User user) =>
            _context.Users.Remove(user);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
