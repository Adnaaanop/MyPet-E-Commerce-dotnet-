using MyApp.Data;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Enhanced GetAllAsync with enum filter + numeric sort
        public async Task<IEnumerable<Order>> GetAllAsync(
            OrderStatus? status = null,
            int? sortId = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Orders
                                .Include(o => o.Items)
                                .AsQueryable();

            // Filter by status if provided
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // Sorting (numeric)
            query = sortId switch
            {
                1 => query.OrderByDescending(o => o.PlacedAt),  // Newest
                2 => query.OrderBy(o => o.PlacedAt),            // Oldest
                3 => query.OrderBy(o => o.Total),               // Total Low
                4 => query.OrderByDescending(o => o.Total),     // Total High
                _ => query.OrderByDescending(o => o.PlacedAt)   // Default: newest
            };

            // Pagination
            query = query.Skip((page - 1) * pageSize)
                         .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                                 .Include(o => o.Items)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                                 .Include(o => o.Items)
                                 .Where(o => o.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
