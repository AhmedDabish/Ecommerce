using backend.DTOs;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IUserRepository _userRepo;
        private readonly IProductRepository _productRepo;
        private readonly IPromoCodeRepository _promoRepo;

        public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo,
                            IUserRepository userRepo, IProductRepository productRepo,
                            IPromoCodeRepository promoRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _promoRepo = promoRepo;
        }

        public async Task<Order> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            var user = await _userRepo.GetUserWithDetailsAsync(userId);
            var address = user.Addresses.FirstOrDefault(a => a.Id == dto.ShippingAddressId);
            if (address == null) throw new Exception("Address not found");

            decimal subTotal = cart.CartItems.Sum(ci => ci.PriceAtAdd * ci.Quantity);
            decimal shippingCost = 50; // À«»  √Ê Õ”» «·„Êﬁ⁄
            decimal taxAmount = subTotal * 0.14m; // 14% VAT
            decimal discountAmount = 0;

            //  ÿ»Ìﬁ ﬂÊœ «·Œ’„ ≈‰ ÊÃœ
            if (dto.PromoCodeId.HasValue)
            {
                var promo = await _promoRepo.GetByIdAsync(dto.PromoCodeId.Value);
                if (promo != null && promo.IsActive && promo.StartDate <= DateTime.UtcNow && promo.EndDate >= DateTime.UtcNow)
                {
                    if (promo.DiscountType == "Percentage")
                        discountAmount = subTotal * (promo.DiscountValue / 100);
                    else
                        discountAmount = promo.DiscountValue;
                    if (discountAmount > subTotal) discountAmount = subTotal;
                    promo.UsedCount++;
                    await _promoRepo.UpdateAsync(promo);
                }
            }

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                SubTotal = subTotal,
                ShippingCost = shippingCost,
                TaxAmount = taxAmount,
                DiscountAmount = discountAmount,
                TotalAmount = subTotal + shippingCost + taxAmount - discountAmount,
                Status = "Pending",
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                ShippingAddressId = dto.ShippingAddressId,
                OrderDate = DateTime.UtcNow,
                Notes = dto.Notes,
                PromoCodeId = dto.PromoCodeId,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.PriceAtAdd,
                    TotalPrice = ci.PriceAtAdd * ci.Quantity
                }).ToList()
            };

            await _orderRepo.AddAsync(order);

            //  ﬁ·Ì· «·ﬂ„Ì… „‰ «·„Œ“Ê‰
            foreach (var item in cart.CartItems)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }

            //  ›—Ì€ «·”·…
            cart.CartItems.Clear();
            await _cartRepo.UpdateAsync(cart);

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int userId, bool isAdmin = false)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null) return null;
            if (!isAdmin && order.UserId != userId) return null;
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
            => await _orderRepo.GetOrdersByUserIdAsync(userId);

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId) return false;
            if (order.Status != "Pending") return false;
            order.Status = "Cancelled";
            await _orderRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;
            order.Status = status;
            if (!string.IsNullOrEmpty(trackingNumber))
                order.TrackingNumber = trackingNumber;
            if (status == "Shipped") order.ShippedDate = DateTime.UtcNow;
            if (status == "Delivered") order.DeliveredDate = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);
            return true;
        }

        private string GenerateOrderNumber()
            => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    }
}