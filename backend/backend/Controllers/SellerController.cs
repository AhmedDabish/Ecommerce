//GET / api / seller / products - منتجات البائع
//GET    /api/seller/orders              - طلبات منتجات البائع
//GET    /api/seller/earnings            - الأرباح
//POST   /api/seller/payout              - طلب سحب

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Repositories.Interfaces;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]
    public class SellerController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public SellerController(IProductRepository productRepo, IOrderRepository orderRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet("products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var products = await _productRepo.FindAsync(p => p.SellerId == GetUserId());
            return Ok(products);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            // جلب الطلبات التي تحتوي على منتجات البائع
            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync(1, 100);
            var filtered = orders.Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == GetUserId()));
            return Ok(filtered);
        }

        [HttpGet("earnings")]
        public async Task<IActionResult> GetEarnings()
        {
            var products = await _productRepo.FindAsync(p => p.SellerId == GetUserId());
            var productIds = products.Select(p => p.Id);
            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync(1, 1000);
            var earnings = orders.Where(o => o.Status == "Delivered")
                                 .SelectMany(o => o.OrderItems)
                                 .Where(oi => productIds.Contains(oi.ProductId))
                                 .Sum(oi => oi.TotalPrice);
            return Ok(new { totalEarnings = earnings, pendingPayouts = 0 });
        }
    }
}