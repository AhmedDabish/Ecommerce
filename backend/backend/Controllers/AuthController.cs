//POST / api / auth / register - تسجيل مستخدم جديد
//POST   /api/auth/login                 - تسجيل الدخول
//POST   /api/auth/confirm-email         - تأكيد البريد الإلكتروني
//POST   /api/auth/forgot-password       - نسيت كلمة المرور
//POST   /api/auth/reset-password        - إعادة تعيين كلمة المرور
//POST   /api/auth/refresh-token         - تحديث الـ Token
//POST   /api/auth/logout                - تسجيل الخروج
//POST   /api/auth/google-login          - (Bonus) تسجيل دخول Google


using backend.DTOs.Auth;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success) return Unauthorized(new { message = result.Message });
            return Ok(new { token = result.Token, user = result.User });
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login(LoginDto dto)
        //{
        //    var result = await _authService.LoginAsync(dto);
        //    if (!result.Success) return Unauthorized(new { message = result.Message });

        //    return Ok(new
        //    {
        //        token = result.Token,
        //        user = result.User,
        //        // Test credentials info
        //        testAccounts = new[]
        //        {
        //    new { role = "Customer", email = "customer@test.com", password = "Test@123" },
        //    new { role = "Seller",   email = "seller@test.com",   password = "Test@123" },
        //    new { role = "Admin",    email = "admin@test.com",    password = "Test@123" }
        //}
        //    });
        //}
        [HttpGet("hash")]
        [AllowAnonymous]
        public IActionResult GetHash()
        {
            return Ok(BCrypt.Net.BCrypt.HashPassword("Test@123"));
        }
    }
}