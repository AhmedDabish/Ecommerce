//using Microsoft.Extensions.Hosting;
//using static System.Net.Mime.MediaTypeNames;

//GET / api / products - قائمة المنتجات(مع Filter & Search & Pagination)
//GET / api / products /{ id}
//-تفاصيل منتج واحد
//GET    /api/products/category/{id}     -منتجات حسب الفئة
//GET    /api/products/featured          - منتجات مميزة
//GET    /api/products/search?q=         - بحث عن منتجات
//POST   /api/products                   - إضافة منتج (Seller/Admin)
//PUT    /api/products/{id}              -تحديث منتج
//DELETE /api/products/{id}              -حذف منتج(Soft Delete)
//POST / api / products /{ id}/ images - رفع صور للمنتج
//DELETE /api/products/images/{id}       -
//حذف صورة



using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}