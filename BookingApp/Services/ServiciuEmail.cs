using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuEmail : IServiciuEmail
    {
        public async Task TrimiteEmailAsync(string laEmail, string subiect, string corp)
        {
            // Logica pentru trimiterea unui email
            // Exemplu: trimitere cu SMTP, SendGrid sau alt serviciu de email
            Console.WriteLine($"Email trimis la {laEmail} cu subiectul: {subiect}");
        }
    }
}
