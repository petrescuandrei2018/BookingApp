using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuHarta
    {
        /// Generează și salvează harta într-un fișier HTML.
        /// <returns>Calea către fișierul HTML generat.</returns>
        Task<string> GenereazaSiSalveazaHarta(string oras, double razaKm);
    }
}
