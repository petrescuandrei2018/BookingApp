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
            console.log('[DEBUG] JSON primit in Mapbox:', data); // Debugging în browser

            var map = new mapboxgl.Map({{
                container: 'map',
                style: 'mapbox://styles/mapbox/streets-v11',
                center: data.coordonateOras ? [data.coordonateOras.Longitudine, data.coordonateOras.Latitudine] : [25.601198, 45.657974],
                zoom: 12
            }});

            if (data.toateHotelurile && Array.isArray(data.toateHotelurile)) {{
                data.toateHotelurile.forEach(hotel => {{
                    if (hotel.Longitudine && hotel.Latitudine) {{
                        new mapboxgl.Marker()
                            .setLngLat([hotel.Longitudine, hotel.Latitudine])
                            .setPopup(new mapboxgl.Popup().setHTML('<b>' + hotel.NumeHotel + '</b>'))
                            .addTo(map);
                    }}
                }});
            }}

            if (data.hoteluriFiltrate && Array.isArray(data.hoteluriFiltrate)) {{
                data.hoteluriFiltrate.forEach(hotel => {{
                    if (hotel.Longitudine && hotel.Latitudine) {{
                        new mapboxgl.Marker({{ color: 'blue' }})
                            .setLngLat([hotel.Longitudine, hotel.Latitudine])
                            .setPopup(new mapboxgl.Popup().setHTML('<b>' + hotel.NumeHotel + '</b>'))
                            .addTo(map);
                    }}
                }});
            }}
        </script>
    </body>
    </html>";
        }
    }
}
