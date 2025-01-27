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
            Console.WriteLine("[SMTP] Începem procesul de trimitere email...");

            try
            {
                // Preluăm configurările din appsettings.json
                var smtpServer = _config["EmailSettings:SmtpServer"];
                var port = int.Parse(_config["EmailSettings:Port"]);
                var username = _config["EmailSettings:Username"];
                var password = _config["EmailSettings:Password"];
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderName = _config["EmailSettings:SenderName"];

                // Log configurare
                Console.WriteLine($"[SMTP] Configurare server: {smtpServer}:{port}");
                Console.WriteLine($"[SMTP] Utilizator: {username}, Expeditor: {senderEmail}");

                // Creăm clientul SMTP
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                // Configurăm mesajul de email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subiect,
                    Body = corp,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(laEmail);

                // Log pentru email
                Console.WriteLine($"[SMTP] Pregătire email către: {laEmail}");
                Console.WriteLine($"[SMTP] Subiect: {subiect}");
                Console.WriteLine($"[SMTP] Corp: {corp}");

                // Trimitem emailul
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"[SMTP] Email trimis cu succes la: {laEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMTP] Eroare la trimiterea emailului: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[SMTP] Detalii suplimentare: {ex.InnerException.Message}");
                }
                throw; // Aruncăm mai departe excepția pentru loguri centralizate
            }
        }
    }
}
