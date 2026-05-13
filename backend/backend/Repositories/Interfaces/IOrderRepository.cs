using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync(int page, int pageSize, string? status = null);
        Task<int> GetPendingOrdersCountAsync();
        Task<decimal> GetTotalSalesAsync();
    }
}