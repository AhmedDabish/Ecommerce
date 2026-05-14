

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment(PaymentRequestDto dto)
        {
            var result = await _paymentService.ProcessPaymentAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("methods")]
        public IActionResult GetPaymentMethods()
        {
            return Ok(new[] { "CreditCard", "PayPal", "COD", "Wallet" });
        }
    }
}