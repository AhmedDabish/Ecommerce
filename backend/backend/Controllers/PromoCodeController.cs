//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Hosting;
//using System.ComponentModel.DataAnnotations;

//namespace backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PromoCodeController : ControllerBase
//    {
//    }
//}


//POST / api / promo / validate - التحقق من كود خصم
//GET    /api/promo                      - كل الأكواد (Admin)
//POST   /api/promo                      - إضافة كود (Admin)
//PUT    /api/promo/{id}                 -تحديث كود
//DELETE /api/promo/{id}                 -
using backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//حذف كود;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IGenericRepository<PromoCode> _promoRepo;

        public PromoCodeController(IGenericRepository<PromoCode> promoRepo)
        {
            _promoRepo = promoRepo;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] string code, [FromQuery] decimal orderAmount)
        {
            var promo = (await _promoRepo.FindAsync(p => p.Code == code && p.IsActive)).FirstOrDefault();
            if (promo == null) return BadRequest(new { message = "Invalid promo code" });
            if (promo.StartDate > DateTime.UtcNow || promo.EndDate < DateTime.UtcNow)
                return BadRequest(new { message = "Promo code expired" });
            if (promo.MinimumOrderAmount.HasValue && orderAmount < promo.MinimumOrderAmount.Value)
                return BadRequest(new { message = $"Minimum order amount is {promo.MinimumOrderAmount.Value}" });
            if (promo.MaxUsageCount.HasValue && promo.UsedCount >= promo.MaxUsageCount.Value)
                return BadRequest(new { message = "Promo code usage limit reached" });

            decimal discount = promo.DiscountType == "Percentage" ? orderAmount * promo.DiscountValue / 100 : promo.DiscountValue;
            return Ok(new { discount, promoId = promo.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var promos = await _promoRepo.GetAllAsync();
            return Ok(promos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PromoCode promo)
        {
            await _promoRepo.AddAsync(promo);
            return Ok(promo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, PromoCode promo)
        {
            if (id != promo.Id) return BadRequest();
            await _promoRepo.UpdateAsync(promo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _promoRepo.GetByIdAsync(id);
            if (promo == null) return NotFound();
            await _promoRepo.DeleteAsync(promo);
            return NoContent();
        }
    }
}