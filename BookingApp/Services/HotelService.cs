using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Linq;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace BookingApp.Services
{
    // Serviciu pentru gestionarea operațiunilor legate de hoteluri
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly IServiciuEmail _serviciuEmail;
        private readonly IServiciuUtilizator _serviciuUtilizator;

        public HotelService(IHotelRepository hotelRepository, IMapper mapper, FabricaServiciuEmail fabricaServiciuEmail, IServiciuUtilizator serviciuUtilizator)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _serviciuEmail = fabricaServiciuEmail.CreeazaServiciuEmail();
            _serviciuUtilizator = serviciuUtilizator;

        }

        public async Task<List<HotelCoordonateDto>> GetAllHotelsCoordonate()
        {
            var hotels = await _hotelRepository.GetAllHotels(); // Obține hotelurile din DB

            return hotels.Select(h => new HotelCoordonateDto
            {
                NumeHotel = h.Name,
                Adresa = h.Address,
                Latitudine = h.Latitudine,
                Longitudine = h.Longitudine
            }).ToList();
        }

        public async Task<bool> VerificaDisponibilitateAsync(int hotelId, int tipCameraId, DateTime checkIn, DateTime checkOut)
        {
            var tipCamere = await _hotelRepository.GetAllTipCamereAsync();

            var tipCamera = tipCamere
                .Where(tc => tc.TipCameraId == tipCameraId && tc.HotelId == hotelId)
                .FirstOrDefault();

            if (tipCamera == null)
            {
                throw new Exception($"Hotelul cu ID {hotelId} nu are camere disponibile înregistrate.");
            }

            var camereOcupate = tipCamera.NrCamereOcupate;

            return camereOcupate < tipCamera.NrTotalCamere;
        }


        public async Task<Rezervare> GetRezervareByIdAsync(int rezervareId)
        {
            return await _hotelRepository.GetRezervareByIdAsync(rezervareId);
        }

        public async Task<List<object>> GetRezervariEligibilePlata()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync(); // Relațiile sunt deja incluse

            var rezultat = new List<object>();

            foreach (var r in rezervari.Where(r => r.Stare == "Viitoare" && r.SumaRamasaDePlata > 0))
            {
                var pretCamera = r.PretCamera;
                var numarNopti = (r.CheckOut - r.CheckIn).Days;
                var sumaTotalaDePlata = pretCamera?.PretNoapte * numarNopti ?? 0;

                rezultat.Add(new
                {
                    r.RezervareId,
                    r.UserId,
                    HotelName = r.PretCamera?.TipCamera?.Hotel?.Name ?? "Hotel necunoscut",
                    CameraName = r.PretCamera?.TipCamera?.Name ?? "Cameră necunoscută",
                    r.CheckIn,
                    r.CheckOut,
                    PretNoapte = pretCamera?.PretNoapte ?? 0,
                    SumaTotalaDePlata = sumaTotalaDePlata,
                    r.SumaAchitata,
                    r.SumaRamasaDePlata
                });
            }

            return rezultat;
        }

        public async Task<List<HotelCuDistantaDto>> ObtineHoteluriPeLocatie(double latitudine, double longitudine, double razaKm)
        {
            var hoteluri = await _hotelRepository.GetAllHotels();

            return hoteluri
                .Select(h => new
                {
                    Hotel = h,
                    Distanta = UtilitatiGeografice.CalculeazaDistanta(latitudine, longitudine, h.Latitudine, h.Longitudine)
                })
                .Where(x => x.Distanta <= razaKm)
                .Select(x => new HotelCuDistantaDto
                {
                    NumeHotel = x.Hotel.Name,
                    Adresa = x.Hotel.Address,
                    DistantaKm = x.Distanta
                })
                .OrderBy(x => x.DistantaKm)
                .ToList();
        }

        public async Task<List<RezervareRefundDto>> GetRezervariEligibileRefund()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync();

            var rezultat = rezervari
                .Where(r => r.StarePlata == "Refundata" || r.SumaAchitata > 0)
                .Select(r => new RezervareRefundDto
                {
                    RezervareId = r.RezervareId,
                    UserId = r.UserId,
                    PaymentIntentId = r.ClientSecret,
                    SumaAchitata = r.SumaAchitata,
                    SumaTotala = r.SumaTotala,
                    SumaRamasaDePlata = r.SumaRamasaDePlata,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                    StarePlata = r.StarePlata
                })
                .ToList();

            return rezultat;
        }

        public async Task TrimiteNotificarePlataIntegralaAsync(int rezervareId, string emailDestinatar)
        {
            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception("Rezervarea nu a fost găsită.");
            }

            if (rezervare.SumaRamasaDePlata > 0)
            {
                throw new Exception("Plata nu este completă. Notificarea nu poate fi trimisă.");
            }

            var mesaj = $"Rezervarea cu ID-ul {rezervare.RezervareId} a fost plătită integral.";
            await _serviciuEmail.TrimiteEmailAsync(emailDestinatar, "Plata completă confirmată", mesaj);
        }

        public async Task TrimiteNotificareRefundAsync(int rezervareId, decimal sumaRefundata, string emailDestinatar)
        {
            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception("Rezervarea nu a fost găsită.");
            }

            var mesaj = $"Suma de {sumaRefundata} RON a fost returnată pentru rezervarea cu ID-ul {rezervare.RezervareId}.";
            await _serviciuEmail.TrimiteEmailAsync(emailDestinatar, "Refund procesat", mesaj);
        }

        public async Task<decimal> ObțineSumaAchitatăAsync(int rezervareId)
        {
            // Obține rezervarea din repository
            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);

            // Returnează suma achitată sau 0 dacă rezervarea nu există
            return rezervare?.SumaAchitata ?? 0;
        }

        public async Task ActualizeazaRezervareAsync(Rezervare rezervare)
        {
            await _hotelRepository.ActualizeazaRezervareAsync(rezervare);
        }

        // Metodă pentru procesarea plății Stripe pentru o rezervare
        public async Task ProcesarePlataStripeAsync(int rezervareId)
        {
            // Obținem rezervarea pe baza ID-ului
            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            // Actualizăm starea plății
            rezervare.StarePlata = "Platita";
            await _hotelRepository.ActualizeazaRezervareAsync(rezervare);
        }

        // Metodă pentru crearea unei rezervări pornind de la DTO
        public async Task<Rezervare> CreateRezervareFromDto(CreareRezervareDto creareRezervareDto, ClaimsPrincipal utilizator)
        {
            if (creareRezervareDto == null)
            {
                throw new ArgumentNullException(nameof(creareRezervareDto), "Datele rezervării nu pot fi null.");
            }

            // 🔹 Obținem ID-ul utilizatorului conectat din token
            var userIdClaim = utilizator.Claims.FirstOrDefault(c => c.Type == "UtilizatorId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Utilizatorul nu este autentificat.");
            }

            creareRezervareDto.UserId = int.Parse(userIdClaim);

            // 🔹 Obținem detaliile camerei
            var pretCamera = await _hotelRepository.GetPretCameraByTipCameraIdAsync(creareRezervareDto.TipCameraId);
            if (pretCamera == null)
            {
                throw new Exception("Camera selectată nu există.");
            }

            // 🔹 Calculăm numărul de nopți și prețul total
            int numarNopti = (creareRezervareDto.CheckOut - creareRezervareDto.CheckIn).Days;
            decimal sumaTotala = (decimal)pretCamera.PretNoapte * numarNopti;

            // 🔹 Creăm obiectul rezervare
            var rezervare = new Rezervare
            {
                UserId = creareRezervareDto.UserId,
                PretCameraId = pretCamera.PretCameraId,
                CheckIn = creareRezervareDto.CheckIn,
                CheckOut = creareRezervareDto.CheckOut,
                Stare = "Viitoare",
                StarePlata = "Neplatita",
                SumaTotala = sumaTotala,
                SumaAchitata = 0,
                SumaRamasaDePlata = sumaTotala
            };

            // 🔹 Salvăm în baza de date prin repository
            await _hotelRepository.AdaugaRezervareAsync(rezervare);
            return rezervare;
        }

        // Obține lista tuturor hotelurilor
        public async Task<List<ResponseHotelDto>> GetAllHotels()
        {
            var hotels = await _hotelRepository.GetAllHotels();
            return _mapper.Map<List<ResponseHotelDto>>(hotels);
        }

        // Obține lista hotelurilor filtrată după nume
        public async Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            if (!string.IsNullOrEmpty(filtruNume))
            {
                hotels = hotels.Where(x => x.Name == filtruNume).ToList();
            }
            return _mapper.Map<List<ResponseHotelDto>>(hotels);
        }

        // Obține lista tuturor rezervărilor
        public async Task<List<GetAllRezervariDto>> GetAllRezervariAsync()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hotels = await _hotelRepository.GetAllHotels();
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var tipCamere = await _hotelRepository.GetAllTipCamereAsync();

            return rezervari.Select(rezervare =>
            {
                var camera = pretCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                var tipCamera = tipCamere.FirstOrDefault(tc => tc.TipCameraId == camera?.TipCameraId);
                var hotel = hotels.FirstOrDefault(h => h.HotelId == tipCamera?.HotelId);

                return new GetAllRezervariDto(
                    rezervare.RezervareId,
                    rezervare.UserId,
                    hotel?.Name ?? "N/A", // 🔹 Evităm valoarea null pentru hotel
                    tipCamera?.Name ?? "N/A", // 🔹 Evităm valoarea null pentru tipul camerei
                    rezervare.CheckIn,
                    rezervare.CheckOut,
                    (decimal?)camera?.PretNoapte ?? 0,
                    rezervare.Stare
                );
            }).ToList();
        }

        // Obține lista hotelurilor împreună cu rating-urile acestora
        public async Task<List<HotelWithRating>> GetAllHotelsByRating(double? rating)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            var recenzii = await _hotelRepository.GetAllRecenzii();

            return hotels.Select(hotel =>
            {
                var hotelRecenzii = recenzii.Where(r => r.HotelId == hotel.HotelId);
                var ratingMediu = hotelRecenzii.Any() ? hotelRecenzii.Average(r => r.Rating) : 0;

                return new HotelWithRating
                {
                    Address = hotel.Address,
                    HotelName = hotel.Name,
                    RecenziiTotale = hotelRecenzii.Count(),
                    Rating = ratingMediu
                };
            })
            .Where(hw => rating == null || hw.Rating >= rating)
            .ToList();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            return await _hotelRepository.GetAllHotelsTipCamera();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere, filtrată
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane)
        {
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane.HasValue)
            {
                hotelsTipCamere = hotelsTipCamere
                    .Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane)
                    .ToList();
            }
            return hotelsTipCamere;
        }

        // Obține lista hotelurilor împreună cu tipurile de camere și prețurile acestora
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            return await _hotelRepository.GetAllHotelsTipCameraPret();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere și prețurile acestora, filtrată
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPretFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pretCamera)
        {
            var hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane.HasValue && pretCamera.HasValue)
            {
                hotelsTipCamerePret = hotelsTipCamerePret
                    .Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane && x.PretNoapte <= pretCamera)
                    .ToList();
            }
            return hotelsTipCamerePret;
        }


        // Obține lista rezervărilor care nu sunt expirate
        public async Task<IEnumerable<GetAllRezervariDto>> GetNonExpiredRezervari()
        {
            // Obține lista de Rezervari din repository
            var rezervari = await _hotelRepository.GetNonExpiredRezervari();

            // Transformă fiecare Rezervare în GetAllRezervariDto
            return rezervari.Select(r =>
            {
                // Accesează relațiile și atribuie valorile
                var hotelName = r.PretCamera?.TipCamera?.Hotel?.Name ?? "Hotel necunoscut";
                var numeCamera = r.PretCamera?.TipCamera?.Name ?? "Tip cameră necunoscut";
                var pret = r.PretCamera?.PretNoapte ?? 0; // Default 0 dacă PretCamera este null

                return new GetAllRezervariDto(
                    r.RezervareId,
                    r.UserId,
                    hotelName,
                    numeCamera,
                    r.CheckIn,
                    r.CheckOut,
                    (decimal)pret, // Conversie din float în decimal
                    r.Stare
                );
            }).ToList();
        }

        public async Task ProcesarePlataPartialaAsync(int rezervareId, decimal sumaAchitata)
        {
            if (sumaAchitata <= 0)
            {
                throw new ArgumentException("Suma achitată trebuie să fie mai mare decât 0.");
            }

            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            if (rezervare.SumaRamasaDePlata <= 0)
            {
                throw new Exception("Rezervarea este deja achitată integral.");
            }

            if (sumaAchitata > rezervare.SumaRamasaDePlata)
            {
                throw new Exception($"Suma achitată depășește suma rămasă de plată. Suma rămasă de achitat este {rezervare.SumaRamasaDePlata}.");
            }

            long sumaInCenti = (long)(sumaAchitata * 100); // Convertim suma în cenți

            try
            {
                // Stripe PaymentIntent logic
                var options = new PaymentIntentCreateOptions
                {
                    Amount = sumaInCenti, // Valoarea plății în cenți
                    Currency = "usd", // Moneda (ajustată după necesități)
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true // Activează metodele automate de plată
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // Confirmarea manuală a PaymentIntent
                var confirmOptions = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = "pm_card_visa" // Exemplu de metodă de plată
                };
                await service.ConfirmAsync(paymentIntent.Id, confirmOptions);

                // Actualizează rezervarea
                rezervare.SumaAchitata += sumaAchitata;
                rezervare.SumaRamasaDePlata -= sumaAchitata;

                // Actualizează StarePlata în funcție de noile valori
                if (rezervare.SumaRamasaDePlata == 0)
                {
                    rezervare.StarePlata = "Platita"; // Marchează plata ca integrală
                }
                else
                {
                    rezervare.StarePlata = "In Progress"; // Încă mai sunt sume de plată
                }

                await _hotelRepository.ActualizeazaRezervareAsync(rezervare);

                // Mesaj succes
                Console.WriteLine($"Plata procesată cu succes. Suma rămasă de plată este {rezervare.SumaRamasaDePlata}.");
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe Error: {ex.StripeError.Message}");
                throw new Exception("A apărut o eroare la procesarea plății.");
            }
        }

        public async Task RefundPaymentAsync(string paymentIntentId, decimal? suma)
        {
            var rezervare = await _hotelRepository.GetRezervareByPaymentIntentAsync(paymentIntentId);
            if (rezervare == null || rezervare.StarePlata != "Platita")
            {
                throw new Exception("Rezervarea nu este eligibilă pentru refund.");
            }

            var service = new Stripe.RefundService();
            await service.CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = suma.HasValue ? (long?)(suma * 100) : null
            });

            rezervare.StarePlata = "Refundata";
            await _hotelRepository.ActualizeazaRezervareAsync(rezervare);
        }

        public async Task<List<HotelCuDistantaDto>> ObtineHoteluriPeOras(string oras, double razaKm)
        {
            // ✅ 1. Obținem coordonatele orașului
            var coordonateOras = await ObțineCoordonateOras(oras);

            // ✅ 2. Dacă orașul nu este găsit, folosim coordonate default și returnăm lista goală
            if (coordonateOras == null)
            {
                Console.WriteLine($"⚠ Orașul {oras} nu a fost găsit în baza de date. Se returnează listă goală.");
                return new List<HotelCuDistantaDto>(); // Fără eroare, dar fără rezultate
            }

            // ✅ 3. Obținem toate hotelurile
            var hoteluri = await _hotelRepository.GetAllHotels();

            // ✅ 4. Calculăm distanța fiecărui hotel față de oraș
            var hoteluriFiltrate = hoteluri
                .Select(h => new
                {
                    Hotel = h,
                    Distanta = UtilitatiGeografice.CalculeazaDistanta(
                        coordonateOras.Latitudine,
                        coordonateOras.Longitudine,
                        h.Latitudine,
                        h.Longitudine)
                })
                .Where(x => x.Distanta <= razaKm) // ✅ Filtrăm doar hotelurile în raza specificată
                .Select(x => new HotelCuDistantaDto
                {
                    NumeHotel = x.Hotel.Name,
                    Adresa = x.Hotel.Address,
                    DistantaKm = x.Distanta,
                    Latitudine = x.Hotel.Latitudine,
                    Longitudine = x.Hotel.Longitudine
                })
                .OrderBy(x => x.DistantaKm)
                .ToList();

            return hoteluriFiltrate;
        }

        public async Task<CoordonateOrasDto?> ObțineCoordonateOras(string oras)
        {
            // Exemplu: apel către un API extern (e.g., OpenCageData, Google Maps)
            // Sau caută în baza ta de date, dacă ai salvat coordonatele orașelor
            var coordonateOrase = new Dictionary<string, (double Latitudine, double Longitudine)>
    {
        { "Bucuresti", (44.4268, 26.1025) },
        { "Brasov", (45.657974, 25.601198) },
        { "Cluj-Napoca", (46.7712, 23.6236) },
        { "Constanta", (44.1598, 28.6348) }
    };

            if (coordonateOrase.TryGetValue(oras, out var coordonate))
            {
                return new CoordonateOrasDto
                {
                    Latitudine = coordonate.Latitudine,
                    Longitudine = coordonate.Longitudine
                };
            }

            return null; // Orașul nu a fost găsit
        }

        public async Task<string> GenereazaHartaAsync(string oras, double razaKm)
        {
            // Obținem coordonatele orașului
            var coordonateOras = await ObțineCoordonateOras(oras);
            if (coordonateOras == null)
            {
                throw new Exception($"Orașul {oras} nu a fost găsit.");
            }

            // Obținem hotelurile din baza de date
            var hoteluri = await ObtineHoteluriPeOras(oras, razaKm);

            // Generăm conținutul HTML
            var htmlContent = @$"
<!DOCTYPE html>
<html>
<head>
    <title>Harta Hoteluri</title>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
    <style>
        #map {{
            height: 600px;
            width: 100%;
        }}
    </style>
</head>
<body>
    <h1>Harta Hotelurilor în {oras}</h1>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([{coordonateOras.Latitudine}, {coordonateOras.Longitudine}], 12);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            maxZoom: 19,
            subdomains: ['a', 'b', 'c'],
            attribution: '© OpenStreetMap contributors'
        }}).addTo(map);

        var hoteluri = {System.Text.Json.JsonSerializer.Serialize(hoteluri)};
        hoteluri.forEach(function(hotel) {{
            L.marker([hotel.latitudine, hotel.longitudine])
             .addTo(map)
             .bindPopup('<b>' + hotel.numeHotel + '</b><br>Distanta: ' + hotel.distantaKm.toFixed(2) + ' km');
        }});
    </script>
</body>
</html>";

            // Salvăm fișierul HTML într-un director temporar
            var fileName = $"HartaHoteluri_{oras}.html";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            await System.IO.File.WriteAllTextAsync(filePath, htmlContent);

            // Returnăm URL-ul relativ sau calea către fișier
            var baseUrl = "http://localhost:5000/Harta"; // Înlocuiește cu URL-ul aplicației tale
            var url = $"{baseUrl}/{fileName}";
            return url;
        }

        public async Task<string> GenereazaHartaHtmlAsync(string oras, double razaKm)
        {
            // Obținem coordonatele orașului
            var coordonateOras = await ObțineCoordonateOras(oras);
            if (coordonateOras == null)
            {
                throw new Exception($"Orașul {oras} nu a fost găsit.");
            }

            // Obținem hotelurile din baza de date
            var hoteluri = await ObtineHoteluriPeOras(oras, razaKm);

            // Generăm conținutul HTML
            var htmlContent = @$"
<!DOCTYPE html>
<html>
<head>
    <title>Harta Hoteluri</title>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
    <style>
        #map {{
            height: 600px;
            width: 100%;
        }}
    </style>
</head>
<body>
    <h1>Harta Hotelurilor în {oras}</h1>
    <div id='map'></div>
    <script>
        var map = L.map('map').setView([{coordonateOras.Latitudine}, {coordonateOras.Longitudine}], 12);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            maxZoom: 19,
            subdomains: ['a', 'b', 'c'],
            attribution: '© OpenStreetMap contributors'
        }}).addTo(map);

        var hoteluri = {System.Text.Json.JsonSerializer.Serialize(hoteluri)};
        hoteluri.forEach(function(hotel) {{
            L.marker([hotel.latitudine, hotel.longitudine])
             .addTo(map)
             .bindPopup('<b>' + hotel.numeHotel + '</b><br>Distanta: ' + hotel.distantaKm.toFixed(2) + ' km');
        }});
    </script>
</body>
</html>";

            return htmlContent;
        }

        public async Task<List<HotelCoordonateDto>> GetAllHotelsCoordonateAsync()
        {
            var hoteluri = await _hotelRepository.GetAllHotels();

            return hoteluri.Select(h => new HotelCoordonateDto
            {
                NumeHotel = h.Name,
                Adresa = h.Address,
                Latitudine = h.Latitudine,
                Longitudine = h.Longitudine
            }).ToList();
        }

        public async Task<List<HotelHartaDto>> GetAllHotelsForMapAsync()
        {
            // Preluăm toate datele înainte de LINQ
            var hoteluri = await _hotelRepository.GetAllHotels();
            var tipuriCamere = await _hotelRepository.GetAllTipCamereAsync();
            var preturiCamere = await _hotelRepository.GetAllPretCamere();
            var recenzii = await _hotelRepository.GetAllRecenzii();

            // Transformăm fiecare hotel în DTO
            var hoteluriDto = hoteluri.Select(hotel => new HotelHartaDto
            {
                HotelId = hotel.HotelId,
                Nume = hotel.Name,
                Adresa = hotel.Address,
                Latitudine = hotel.Latitudine,
                Longitudine = hotel.Longitudine,

                // Obținem tipurile de camere pentru hotel
                TipuriCamere = tipuriCamere
                    .Where(tc => tc.HotelId == hotel.HotelId)
                    .Select(tc => tc.Name)
                    .ToList(),

                // Calculăm prețul mediu pe noapte
                PretMediuNoapte = preturiCamere
                    .Where(pc => tipuriCamere
                        .Where(tc => tc.HotelId == hotel.HotelId)
                        .Select(tc => tc.TipCameraId)
                        .Contains(pc.TipCameraId))
                    .Average(pc => (decimal?)pc.PretNoapte) ?? 0,

                // Calculăm rating-ul mediu
                RatingMediu = recenzii
                    .Where(r => r.HotelId == hotel.HotelId)
                    .Average(r => (double?)r.Rating) ?? 0,

                // Imagine placeholder
                ImagineUrl = $"https://source.unsplash.com/400x300/?hotel,city"
            }).ToList();

            return hoteluriDto;
        }
    }
}
