using System.Net;
using System.Net.Mail;
using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BookingApp.Services
{
    public class ServiciuEmailSmtp : IServiciuEmail
    {
        private readonly IConfiguration _config;

        public ServiciuEmailSmtp(IConfiguration config)
        {
            _config = config;
        }

        public async Task TrimiteEmailAsync(string laEmail, string subiect, string corp)
        {
            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]),
                Subject = subiect,
                Body = corp,
                IsBodyHtml = true
            };
            mailMessage.To.Add(laEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
