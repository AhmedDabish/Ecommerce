using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace backend.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _config;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendConfirmationEmail(string toEmail, string token)
        {
            var link = $"http://localhost:4200/auth/confirm-email?token={token}";
            var body = $"<a href='{link}'>اضغط هنا لتأكيد البريد</a>";
            await SendEmail(toEmail, "تأكيد البريد الإلكتروني", body);
        }

        private async Task SendEmail(string to, string subject, string body)
        {
            using var client = new SmtpClient(_config["Email:Host"], int.Parse(_config["Email:Port"]))
            {
                Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
                EnableSsl = true
            };
            var mail = new MailMessage(_config["Email:From"], to, subject, body) { IsBodyHtml = true };
            await client.SendMailAsync(mail);
        }
    }
}