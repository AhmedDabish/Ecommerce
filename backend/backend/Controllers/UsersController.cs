//GET / api / users / profile - الحصول على بيانات المستخدم
//PUT    /api/users/profile              - تحديث البيانات الشخصية
//PUT    /api/users/change-password      - تغيير كلمة المرور
//GET    /api/users/addresses            - قائمة العناوين
//POST   /api/users/addresses            - إضافة عنوان جديد
//PUT    /api/users/addresses/{id}       -تحديث عنوان
//DELETE /api/users/addresses/{id}       -حذف عنوان
//GET    /api/users/orders               - تاريخ الطلبات
//GET    /api/users/wallet
//- (Bonus) رصيد المحفظة

using backend.DTOs;
using backend.Repositories.Interfaces;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IUserRepository _userRepo;
        private readonly IAddressRepository _addressRepo;

        public UsersController(UserService userService, IUserRepository userRepo, IAddressRepository addressRepo)
        {
            _userService = userService;
            _userRepo = userRepo;
            _addressRepo = addressRepo;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userRepo.GetUserWithDetailsAsync(GetUserId());
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var user = await _userService.UpdateProfileAsync(GetUserId(), dto);
            return Ok(user);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var result = await _userService.ChangePasswordAsync(GetUserId(), dto);
            if (!result) return BadRequest(new { message = "Old password is incorrect" });
            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var user = await _userRepo.GetUserWithDetailsAsync(GetUserId());
            return Ok(user.Addresses);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(CreateAddressDto dto)
        {
            var user = await _userRepo.GetUserWithDetailsAsync(GetUserId());
            var address = new Address
            {
                UserId = GetUserId(),
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country,
                IsDefault = dto.IsDefault
            };
            if (address.IsDefault)
            {
                foreach (var a in user.Addresses) a.IsDefault = false;
            }
            await _addressRepo.AddAsync(address);
            return Ok(address);
        }

        [HttpPut("addresses/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, CreateAddressDto dto)
        {
            var address = await _addressRepo.GetByIdAsync(id);
            if (address == null || address.UserId != GetUserId()) return NotFound();
            address.Street = dto.Street;
            address.City = dto.City;
            address.State = dto.State;
            address.ZipCode = dto.ZipCode;
            address.Country = dto.Country;
            address.IsDefault = dto.IsDefault;
            await _addressRepo.UpdateAsync(address);
            return Ok(address);
        }

        [HttpDelete("addresses/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _addressRepo.GetByIdAsync(id);
            if (address == null || address.UserId != GetUserId()) return NotFound();
            await _addressRepo.DeleteAsync(address);
            return NoContent();
        }

        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var user = await _userRepo.GetByIdAsync(GetUserId());
            return Ok(new { balance = user.WalletBalance });
        }
    }
}