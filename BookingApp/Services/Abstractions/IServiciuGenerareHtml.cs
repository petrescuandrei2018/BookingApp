using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuGenerareHtml
    {
        Task<string> GenereazaHtmlHarta(string jsonHoteluri);
    }
}
