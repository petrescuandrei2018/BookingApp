using BookingApp.Services.Abstractions;
using System.Net.Mail;

namespace BookingApp.Services
{
    public class ServiciuEmailMock : IServiciuEmail
    {
        public async Task TrimiteEmailAsync(string laEmail, string subiect, string corp)
        {
            // Emulare trimitere email
            Console.WriteLine($"[Mock] Email trimis la: {laEmail}");
            Console.WriteLine($"[Mock] Subiect: {subiect}");
            Console.WriteLine($"[Mock] Corp: {corp}");
            await Task.CompletedTask; // Simulare operațiune asincronă
        }
    }
}
