using System.Threading.Tasks;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuGenerareHtml : IServiciuGenerareHtml
    {
        public async Task<string> GenereazaHtmlHarta(string jsonHoteluri)
        {
            return $@"
    <!DOCTYPE html>
    <html>
    <head>
        <title>Harta Hoteluri</title>
        <meta charset='utf-8' />
        <script src='https://api.mapbox.com/mapbox-gl-js/v2.6.1/mapbox-gl.js'></script>
        <link href='https://api.mapbox.com/mapbox-gl-js/v2.6.1/mapbox-gl.css' rel='stylesheet' />
        <style>#map {{ height: 600px; width: 100%; }}</style>
    </head>
    <body>
        <h1>Harta Hotelurilor</h1>
        <div id='map'></div>
        <script>
            mapboxgl.accessToken = 'pk.eyJ1IjoicGV0cmVzY3VhbmRyZWkyMDE4IiwiYSI6ImNtNmp0ZXIwMDA1ZDQyanNoeWI0NHp4MGoifQ.O_k6-YsjhtV6e_K75h3Jrg';
            var data = {jsonHoteluri};
            
            var map = new mapboxgl.Map({{
                container: 'map',
                style: 'mapbox://styles/mapbox/streets-v11',
                center: [data.coordonateOras.Longitudine, data.coordonateOras.Latitudine],
                zoom: 12
            }});



            data.toateHotelurile.forEach(function(hotel) {{
                new mapboxgl.Marker()
                    .setLngLat([hotel.Longitudine, hotel.Latitudine])
                    .setPopup(new mapboxgl.Popup().setHTML('<b>' + hotel.NumeHotel + '</b><br>' + hotel.Adresa))
                    .addTo(map);
            }});

            data.hoteluriFiltrate.forEach(function(hotel) {{
                new mapboxgl.Marker({{
                    color: 'blue'
                }})
                .setLngLat([hotel.Longitudine, hotel.Latitudine])
                .setPopup(new mapboxgl.Popup().setHTML('<b>' + hotel.NumeHotel + '</b><br>' + hotel.Adresa))
                .addTo(map);
            }});
        </script>
    </body>
    </html>";
        }
    }
}
