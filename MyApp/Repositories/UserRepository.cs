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

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.Users.ToListAsync();

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
