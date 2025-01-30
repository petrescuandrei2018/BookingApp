using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookingApp.Services
{
    public class ServiciuCoordonate : IServiciuCoordonate
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiciuCoordonate> _logger;
        private readonly string _apiKey;

        public ServiciuCoordonate(HttpClient httpClient, ILogger<ServiciuCoordonate> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = config["OpenWeather:ApiKey"]; // ✅ Citim API Key-ul din config

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key pentru OpenWeather lipsește.");
            }
        }

        public async Task<CoordonateOrasDto?> ObtineCoordonateOras(string oras)
        {
            try
            {
                string url = $"https://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(oras)}&limit=1&appid=aaf04cce6c826395586b11bd53674b59";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] OpenWeather API nu a returnat date valide pentru {oras}. Cod HTTP: {response.StatusCode}");
                    return new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 }; // Default Brașov
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var rezultat = JsonSerializer.Deserialize<List<OpenWeatherResponse>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (rezultat == null || rezultat.Count == 0 || rezultat[0].Lat == 0 || rezultat[0].Lon == 0)
                {
                    Console.WriteLine($"[WARN] OpenWeather nu a returnat coordonate pentru {oras}. Se folosește fallback.");
                    return new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 };
                }

                return new CoordonateOrasDto
                {
                    Latitudine = rezultat[0].Lat,
                    Longitudine = rezultat[0].Lon
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Eroare la obținerea coordonatelor pentru {oras}: {ex.Message}");
                return new CoordonateOrasDto { Latitudine = 45.657974, Longitudine = 25.601198 }; // Default Brașov
            }
        }

        private class OpenWeatherResponse
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }

    // ✅ Model pentru OpenWeather API
    public class OpenWeatherResponse
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
