//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace backend.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class WishlistController : ControllerBase
//    {


//        GET    /api/wishlist                   - عرض المفضلة
//POST   /api/wishlist/items             - إضافة منتج للمفضلة
//DELETE /api/wishlist/items/{id
//    }        - حذف من المفضلة


//}
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var wishlist = await _wishlistService.GetWishlistAsync(GetUserId());
            return Ok(wishlist.Items.Select(i => i.Product));
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToWishlist([FromBody] int productId)
        {
            await _wishlistService.AddToWishlistAsync(GetUserId(), productId);
            return Ok(new { message = "Added to wishlist" });
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            await _wishlistService.RemoveFromWishlistAsync(GetUserId(), productId);
            return NoContent();
        }
    }
}