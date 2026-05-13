using backend.DTOs.Auth;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Helpers;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailHelper _emailHelper;

        public AuthService(IUserRepository userRepo, JwtHelper jwtHelper, EmailHelper emailHelper)
        {
            _userRepo = userRepo;
            _jwtHelper = jwtHelper;
            _emailHelper = emailHelper;
        }

        public async Task<(bool Success, string Message, User? User, string? Token)> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.ExistsAsync(u => u.Email == dto.Email))
                return (false, "البريد الإلكتروني موجود بالفعل", null, null);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.Role == "Seller" ? 2 : 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailConfirmed = false,
                EmailConfirmationToken = GenerateRandomToken()
            };

            await _userRepo.AddAsync(user);
            //await _emailHelper.SendConfirmationEmail(user.Email, user.EmailConfirmationToken);
            return (true, "تم التسجيل بنجاح، يرجى تأكيد البريد", user, null);
        }

        public async Task<(bool Success, string Message, User? User, string? Token)> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                return (false, "البريد أو كلمة المرور غير صحيحة", null, null);
            if (!user.IsActive)
                return (false, "الحساب غير نشط", null, null);
            //if (!user.IsEmailConfirmed)
            //    return (false, "يرجى تأكيد البريد الإلكتروني أولاً", null, null);

            user.LastLogin = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);

            var token = _jwtHelper.GenerateToken(user);
            return (true, "تم تسجيل الدخول بنجاح", user, token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;

        private string GenerateRandomToken()
            => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}