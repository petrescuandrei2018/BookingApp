using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuSalvareHarta
    {
        Task<string> SalveazaHartaHtml(string oras, string htmlContent);
    }
}
