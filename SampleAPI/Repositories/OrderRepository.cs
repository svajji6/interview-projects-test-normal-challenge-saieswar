using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _context;

        public OrderRepository(SampleApiDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetRecentOrders()
        {
            return await _context.Orders
                .Where(o => !o.IsDeleted && o.EntryDate > DateTime.UtcNow.AddDays(-1))
                .OrderByDescending(o => o.EntryDate)
                .ToListAsync();
        }
        public async Task AddNewOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersFromDate(DateTime startDate)
        {
            return await _context.Orders
                .Where(o => o.EntryDate >= startDate && !o.IsDeleted)
                .OrderByDescending(o => o.EntryDate)
                .ToListAsync();
        }

    }
}
