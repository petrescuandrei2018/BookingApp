using System.Text.Json;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuHarta : IServiciuHarta
    {
        private readonly IServiciuFiltrareHoteluri _serviciuFiltrare;
        private readonly IServiciuGenerareHtml _serviciuGenerareHtml;
        private readonly IServiciuSalvareHarta _serviciuSalvareHarta;
        private readonly IServiciuCoordonate _serviciuCoordonate; // 🔹 Adăugat

        public ServiciuHarta(
            IServiciuFiltrareHoteluri serviciuFiltrare,
            IServiciuGenerareHtml serviciuGenerareHtml,
            IServiciuSalvareHarta serviciuSalvareHarta,
            IServiciuCoordonate serviciuCoordonate) // 🔹 Adăugat în constructor
        {
            _serviciuFiltrare = serviciuFiltrare;
            _serviciuGenerareHtml = serviciuGenerareHtml;
            _serviciuSalvareHarta = serviciuSalvareHarta;
            _serviciuCoordonate = serviciuCoordonate; // 🔹 Inițializat
        }

        public async Task<string> GenereazaSiSalveazaHarta(string oras, double razaKm)
        {
            var toateHotelurile = await _serviciuFiltrare.FiltreazaHoteluri("", 0);
            var hoteluriFiltrate = await _serviciuFiltrare.FiltreazaHoteluri(oras, razaKm);

            var coordonateOras = await _serviciuCoordonate.ObtineCoordonateOras(oras)
                              ?? new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 }; // Default Brașov

            if (coordonateOras == null || coordonateOras.Latitudine == 0 || coordonateOras.Longitudine == 0)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut obține coordonatele pentru orașul {oras}. Se folosește fallback.");
                coordonateOras = new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 }; // Default Brașov
            }

            // 🔹 Debugging: Verificăm datele înainte de generarea HTML
            Console.WriteLine($"[DEBUG] JSON generat pentru hartă: {JsonSerializer.Serialize(new { toateHotelurile, hoteluriFiltrate, coordonateOras }, new JsonSerializerOptions { WriteIndented = true })}");

            var jsonHoteluri = System.Text.Json.JsonSerializer.Serialize(new
            {
                toateHotelurile = toateHotelurile.Select(h => new
                {
                    h.NumeHotel,
                    h.Latitudine,
                    h.Longitudine
                }).ToList(),

                hoteluriFiltrate = hoteluriFiltrate.Select(h => new
                {
                    h.NumeHotel,
                    h.Latitudine,
                    h.Longitudine
                }).ToList(),

                coordonateOras
            });

            var htmlContent = await _serviciuGenerareHtml.GenereazaHtmlHarta(jsonHoteluri);
            return await _serviciuSalvareHarta.SalveazaHartaHtml(oras, htmlContent);
        }
    }
}
