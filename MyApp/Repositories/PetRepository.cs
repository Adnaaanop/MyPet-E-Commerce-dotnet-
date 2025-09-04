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
