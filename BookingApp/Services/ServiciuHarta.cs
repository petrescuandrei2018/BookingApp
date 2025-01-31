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
        private readonly IServiciuCoordonate _serviciuCoordonate;
        private readonly IHotelService _hotelService; // 🔹 Adăugat

        public ServiciuHarta(
            IServiciuFiltrareHoteluri serviciuFiltrare,
            IServiciuGenerareHtml serviciuGenerareHtml,
            IServiciuSalvareHarta serviciuSalvareHarta,
            IServiciuCoordonate serviciuCoordonate,
            IHotelService hotelService) // 🔹 Adăugat în constructor
        {
            _serviciuFiltrare = serviciuFiltrare;
            _serviciuGenerareHtml = serviciuGenerareHtml;
            _serviciuSalvareHarta = serviciuSalvareHarta;
            _serviciuCoordonate = serviciuCoordonate;
            _hotelService = hotelService; // 🔹 Inițializat
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



/*using System.Text.Json;
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
        private readonly IServiciuCoordonate _serviciuCoordonate;
        private readonly IHotelService _hotelService;

        public ServiciuHarta(
            IServiciuFiltrareHoteluri serviciuFiltrare,
            IServiciuGenerareHtml serviciuGenerareHtml,
            IServiciuSalvareHarta serviciuSalvareHarta,
            IServiciuCoordonate serviciuCoordonate,
            IHotelService hotelService)
        {
            _serviciuFiltrare = serviciuFiltrare;
            _serviciuGenerareHtml = serviciuGenerareHtml;
            _serviciuSalvareHarta = serviciuSalvareHarta;
            _serviciuCoordonate = serviciuCoordonate;
            _hotelService = hotelService;
        }

        public async Task<string> GenereazaSiSalveazaHarta(string oras, double razaKm)
        {
            // ✅ Obținem doar hotelurile din raza specificată (fără `toateHotelurile`)
            var hoteluriFiltrate = await _serviciuFiltrare.FiltreazaHoteluri(oras, razaKm);

            // ✅ Dacă nu există hoteluri, folosim fallback
            if (!hoteluriFiltrate.Any())
            {
                Console.WriteLine($"[AVERTIZARE] Nu s-au găsit hoteluri pentru {oras}. Se va folosi un marker default.");
                hoteluriFiltrate = new List<HotelCoordonateDto>
        {
            new HotelCoordonateDto
            {
                NumeHotel = "Hotel Fallback",
                Latitudine = 45.657974,
                Longitudine = 25.601198
            }
        };
            }

            // ✅ Obținem coordonatele orașului (fallback pe Brașov dacă nu există)
            var coordonateOras = await _serviciuCoordonate.ObtineCoordonateOras(oras)
                              ?? new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 };

            if (coordonateOras.Latitudine == 0 || coordonateOras.Longitudine == 0)
            {
                Console.WriteLine($"[EROARE] Nu s-au putut obține coordonatele pentru {oras}. Se folosește fallback.");
                coordonateOras = new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 };
            }

            // ✅ JSON mai sigur (verificare că variabilele există)
            var jsonHoteluri = JsonSerializer.Serialize(new
            {
                hoteluriFiltrate = hoteluriFiltrate.Select(h => new
                {
                    h.NumeHotel,
                    h.Latitudine,
                    h.Longitudine
                }).ToList()
            });

            // ✅ Generare HTML optimizată
            var htmlContent = await _serviciuGenerareHtml.GenereazaHtmlHarta(jsonHoteluri);

            // ✅ Salvăm fișierul hărții și returnăm linkul de descărcare
            return await _serviciuSalvareHarta.SalveazaHartaHtml(oras, htmlContent);
        }
    }
}
*/