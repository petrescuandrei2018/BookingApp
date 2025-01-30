using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuHarta
    {
        /// <summary>
        /// Generează și salvează harta într-un fișier HTML.
        /// </summary>
        /// <param name="oras">Orașul pentru care se generează harta.</param>
        /// <param name="razaKm">Raza de căutare în km.</param>
        /// <returns>Calea către fișierul HTML generat.</returns>
        Task<string> GenereazaSiSalveazaHarta(string oras, double razaKm);
    }
}
