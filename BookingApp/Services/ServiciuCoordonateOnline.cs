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
    public class ServiciuCoordonateOnline : IServiciuCoordonateOnline
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiciuCoordonateOnline> _logger;
        private readonly string _apiKey;

        public ServiciuCoordonateOnline(HttpClient httpClient, ILogger<ServiciuCoordonateOnline> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = config["OpenWeather:ApiKey"]; // Citim API Key-ul din config

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key pentru OpenWeather lipsește.");
            }
        }

        public async Task<CoordonateOrasDto?> ObtineCoordonateOras(string oras)
        {
            try
            {
                string url = $"https://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(oras)}&limit=1&appid={_apiKey}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "BookingApp"); // OpenWeather blochează request-urile fără User-Agent

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogError($"[API ERROR] OpenWeather API a returnat 403 Forbidden pentru orașul {oras}");
                    throw new Exception("Acces interzis la OpenWeather API. Verifică cheia API.");
                }

                response.EnsureSuccessStatusCode(); // Aruncă excepție dacă nu este succes

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var rezultat = JsonSerializer.Deserialize<List<OpenWeatherResponse>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (rezultat == null || rezultat.Count == 0)
                {
                    _logger.LogWarning($"[API WARN] Nu s-au găsit coordonate pentru orașul: {oras}");
                    return null;
                }

                return new CoordonateOrasDto
                {
                    Latitudine = rezultat[0].Lat,
                    Longitudine = rezultat[0].Lon
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] Eroare la obținerea coordonatelor pentru {oras}: {ex.Message}");
                return null;
            }
        }

        private class OpenWeatherResponse
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
