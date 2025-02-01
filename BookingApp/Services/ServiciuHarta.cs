using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Services
{
    public class ServiciuHarta : IServiciuHarta
    {
        private readonly IServiciuFiltrareHoteluri _serviciuFiltrare;
        private readonly IServiciuGenerareHtml _serviciuGenerareHtml;
        private readonly IServiciuSalvareHarta _serviciuSalvareHarta;
        private readonly IServiciuCoordonate _serviciuCoordonate;
        private readonly IHotelService _serviciuHotel; // 🔹 Adăugat


        public ServiciuHarta(
            IServiciuFiltrareHoteluri serviciuFiltrare,
            IServiciuGenerareHtml serviciuGenerareHtml,
            IServiciuSalvareHarta serviciuSalvareHarta,
            IServiciuCoordonate serviciuCoordonate,
            IHotelService serviciuHotel)
        {
            _serviciuFiltrare = serviciuFiltrare;
            _serviciuGenerareHtml = serviciuGenerareHtml;
            _serviciuSalvareHarta = serviciuSalvareHarta;
            _serviciuCoordonate = serviciuCoordonate;
            _serviciuHotel = serviciuHotel; // 🔹 Inițializare

        }

        public async Task<FileResult> GenereazaSiSalveazaHarta(string oras, double razaKm)
        {
            Console.WriteLine($"[INFO] Generare hartă pentru oraș: {oras}, raza: {razaKm} km");

            // 🔹 Obține toate hotelurile din baza de date
            var toateHotelurile = await _serviciuHotel.GetAllHotelsCoordonateAsync();

            // 🔹 Obține doar hotelurile din raza definită de utilizator
            var hoteluriFiltrate = await _serviciuFiltrare.FiltreazaHoteluri(oras, razaKm);

            // 🔹 Încearcă să obții coordonatele orașului din baza de date
            var coordonateOras = await _serviciuCoordonate.ObtineCoordonateOras(oras);

            if (coordonateOras == null)
            {
                Console.WriteLine($"[WARN] Orașul '{oras}' nu a fost găsit în baza de date. Se încearcă obținerea coordonatelor externe...");

                // 🔹 Căutăm coordonatele orașului folosind OpenStreetMap
                coordonateOras = await _serviciuCoordonate.ObtineCoordonateOrasExterne(oras);

                if (coordonateOras == null)
                {
                    Console.WriteLine($"[EROARE] Orașul '{oras}' nu a fost găsit nici extern.");

                    // 🔹 Dacă orașul nu este găsit deloc, setăm un punct default (România)
                    coordonateOras = new CoordonateOrasDto
                    {
                        NumeOras = "România",
                        Latitudine = 45.9432,
                        Longitudine = 24.9668
                    };
                }
            }

            // 🔹 Serializăm TOATE hotelurile + cele filtrate
            var jsonHoteluri = JsonSerializer.Serialize(new
            {
                hoteluriFiltrate = hoteluriFiltrate.Select(h => new
                {
                    h.NumeHotel,
                    h.Latitudine,
                    h.Longitudine
                }).ToList(),
                toateHotelurile = toateHotelurile.Select(h => new
                {
                    h.NumeHotel,
                    h.Latitudine,
                    h.Longitudine
                }).ToList(),
                coordonateOras
            });

            var htmlContent = await _serviciuGenerareHtml.GenereazaHtmlHarta(jsonHoteluri);
            var caleFisier = await _serviciuSalvareHarta.SalveazaHartaHtml(oras, htmlContent);
            var fileBytes = await File.ReadAllBytesAsync(caleFisier);

            return new FileContentResult(fileBytes, "text/html")
            {
                FileDownloadName = $"HartaHoteluri_{oras}.html"
            };
        }
    }
}
