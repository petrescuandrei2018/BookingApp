using System.Text.Json;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuGenerareHarta : IServiciuGenerareHarta
    {
        private readonly IHotelService _serviciuHotel;

        public ServiciuGenerareHarta(IHotelService serviciuHotel)
        {
            _serviciuHotel = serviciuHotel;
        }

        public async Task<string> GenereazaHartaHtml(string? oras, double? razaKm)
        {
            // 1️⃣ Obținem toate hotelurile
            var toateHotelurile = await _serviciuHotel.GetAllHotelsCoordonate();
            List<HotelCoordonateDto> hoteluriFiltrate = new();
            CoordonateOrasDto coordonateOras;

            // 2️⃣ Obținem coordonatele orașului (fallback dacă nu există)
            if (!string.IsNullOrEmpty(oras))
            {
                var coordonate = await _serviciuHotel.ObțineCoordonateOras(oras);
                coordonateOras = coordonate ?? new CoordonateOrasDto { Latitudine = 46.0, Longitudine = 25.0 };

                if (coordonate != null && razaKm.HasValue)
                {
                    var hoteluriCuDistanta = await _serviciuHotel.ObtineHoteluriPeOras(oras, razaKm.Value);
                    hoteluriFiltrate = hoteluriCuDistanta.Select(h => new HotelCoordonateDto
                    {
                        NumeHotel = h.NumeHotel,
                        Adresa = h.Adresa,
                        Latitudine = h.Latitudine,
                        Longitudine = h.Longitudine
                    }).ToList();
                }
            }
            else
            {
                coordonateOras = new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 };
            }

            // 3️⃣ Generăm HTML-ul
            var htmlContent = $@"
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
    <h1>Harta Hotelurilor</h1>
    <input type='text' id='orasInput' placeholder='Introdu orașul' value='{oras ?? ""}'>
    <input type='number' id='razaInput' placeholder='Raza (km)' value='{razaKm ?? 10}'>
    <button onclick='filtreazaHoteluri()'>Filtrează</button>
    <div id='map'></div>

    <script>
        var map = L.map('map').setView([{coordonateOras.Latitudine}, {coordonateOras.Longitudine}], 12);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            maxZoom: 19,
            subdomains: ['a', 'b', 'c'],
            attribution: '© OpenStreetMap contributors'
        }}).addTo(map);

        var allMarkers = L.featureGroup().addTo(map);
        var filteredMarkers = L.featureGroup().addTo(map);

        function updateMap(data) {{
            allMarkers.clearLayers();
            filteredMarkers.clearLayers();
            map.setView([data.coordonateOras.latitudine, data.coordonateOras.longitudine], 12);

            data.toateHotelurile.forEach(function(hotel) {{
                var marker = L.marker([hotel.latitudine, hotel.longitudine], {{
                    icon: L.icon({{
                        iconUrl: 'https://maps.google.com/mapfiles/ms/icons/red-dot.png',
                        iconSize: [32, 32],
                        iconAnchor: [16, 32]
                    }})
                }})
                .bindPopup('<b>' + hotel.numeHotel + '</b><br>' + hotel.adresa);
                allMarkers.addLayer(marker);
            }});

            data.hoteluriFiltrate.forEach(function(hotel) {{
                var marker = L.marker([hotel.latitudine, hotel.longitudine], {{
                    icon: L.icon({{
                        iconUrl: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png',
                        iconSize: [32, 32],
                        iconAnchor: [16, 32]
                    }})
                }})
                .bindPopup('<b>' + hotel.numeHotel + '</b><br>' + hotel.adresa);
                filteredMarkers.addLayer(marker);
            }});

            if (allMarkers.getBounds().isValid()) {{
                map.fitBounds(allMarkers.getBounds());
            }}
        }}

        function filtreazaHoteluri() {{
            var oras = document.getElementById('orasInput').value;
            var raza = document.getElementById('razaInput').value;

            fetch(`/api/Hotel/GenereazaHarta?oras=${{encodeURIComponent(oras)}}&razaKm=${{encodeURIComponent(raza)}}`)
            .then(response => response.text())
            .then(html => {{
                document.open();
                document.write(html);
                document.close();
            }})
            .catch(error => {{
                console.error('Eroare la încărcarea datelor:', error);
                alert('A apărut o eroare. Încearcă din nou.');
            }});
        }}

        updateMap({{
            toateHotelurile: {JsonSerializer.Serialize(toateHotelurile)},
            hoteluriFiltrate: {JsonSerializer.Serialize(hoteluriFiltrate)},
            coordonateOras: {JsonSerializer.Serialize(coordonateOras)}
        }});
    </script>
</body>
</html>";

            return htmlContent;
        }
    }
}
