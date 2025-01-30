using System.IO;
using System.Threading.Tasks;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuSalvareHarta : IServiciuSalvareHarta
    {
        public async Task<string> SalveazaHartaHtml(string oras, string htmlContent)
        {
            var fileName = $"HartaHoteluri_{oras.Replace(" ", "_")}.html"; // Înlocuiește spațiile pentru siguranță
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            await System.IO.File.WriteAllTextAsync(filePath, htmlContent);
            return filePath;
        }
    }
}
