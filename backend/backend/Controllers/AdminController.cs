
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Repositories.Interfaces;
using backend.Services;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly OrderService _orderService;

        public AdminController(IUserRepository userRepo, IOrderRepository orderRepo, IProductRepository productRepo, OrderService orderService)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _orderService = orderService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(users);
        }

        [HttpPut("users/{id}/approve")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = true;
            await _userRepo.UpdateAsync(user);
            return Ok(new { message = "User approved" });
        }

        [HttpPut("users/{id}/restrict")]
        public async Task<IActionResult> RestrictUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = false;
            await _userRepo.UpdateAsync(user);
            return Ok(new { message = "User restricted" });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null)
        {
            var orders = await _orderRepo.GetAllOrdersWithDetailsAsync(page, pageSize, status);
            return Ok(orders);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status, dto.TrackingNumber);
            if (!result) return NotFound();
            return Ok(new { message = "Order status updated" });
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalSales = await _orderRepo.GetTotalSalesAsync();
            var totalOrders = await _orderRepo.CountAsync();
            var totalUsers = await _userRepo.CountAsync();
            var totalProducts = await _productRepo.CountAsync(p => p.IsActive);
            var pendingOrders = await _orderRepo.GetPendingOrdersCountAsync();
            var recentOrders = (await _orderRepo.GetAllOrdersWithDetailsAsync(1, 5)).Select(o => new RecentOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderDate = o.OrderDate
            });

            var dashboard = new AdminDashboardDto
            {
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                PendingOrders = pendingOrders,
                RecentOrders = recentOrders.ToList()
            };
            return Ok(dashboard);
        }
    }
}