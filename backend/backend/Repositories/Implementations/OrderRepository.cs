using backend.Data;
using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
            => await _dbSet.Where(o => o.UserId == userId)
                           .OrderByDescending(o => o.OrderDate)
                           .Include(o => o.OrderItems)
                           .ThenInclude(oi => oi.Product)
                           .ThenInclude(p => p.Images)
                           .ToListAsync();

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
            => await _dbSet.Include(o => o.OrderItems)
                           .ThenInclude(oi => oi.Product)
                           .ThenInclude(p => p.Images)
                           .Include(o => o.ShippingAddress)
                           .Include(o => o.Payment)
                           .FirstOrDefaultAsync(o => o.Id == orderId);

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync(int page, int pageSize, string? status = null)
        {
            var query = _dbSet.Include(o => o.User)
                              .Include(o => o.OrderItems)
                              .AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);
            return await query.OrderByDescending(o => o.OrderDate)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
            => await _dbSet.CountAsync(o => o.Status == "Pending");

        public async Task<decimal> GetTotalSalesAsync()
            => await _dbSet.Where(o => o.Status == "Delivered" || o.Status == "Shipped")
                           .SumAsync(o => o.TotalAmount);
    }
}