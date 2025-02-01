using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;

public class ServiciuCoordonate : IServiciuCoordonate
{
    private readonly HttpClient _httpClient;

    public ServiciuCoordonate(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CoordonateOrasDto?> ObtineCoordonateOras(string oras)
    {
        try
        {
            Console.WriteLine($"[INFO] Verificare oraș: {oras}");

            string url = $"https://api.openweathermap.org/geo/1.0/direct?q={oras}&limit=1&appid=YOUR_API_KEY";
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EROARE] API-ul OpenWeather a răspuns cu {response.StatusCode}");
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            var rezultat = JsonSerializer.Deserialize<List<CoordonateOrasDto>>(responseBody);

            if (rezultat != null && rezultat.Count > 0)
            {
                return rezultat.First();
            }
            else
            {
                string orasSugerat = await GasesteOrasApropiat(oras);
                throw new Exception($"Orașul '{oras}' nu a fost găsit. Ai vrut să spui '{orasSugerat}'?");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EROARE] Nu s-au putut obține coordonatele: {ex.Message}");
            return null;
        }
    }

    public async Task<string> GasesteOrasApropiat(string orasIntrodus)
    {
        string url = $"https://api.openweathermap.org/geo/1.0/direct?q={orasIntrodus}&limit=5&appid=YOUR_API_KEY";
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[EROARE] Nu am putut obține sugestii de orașe. API a răspuns cu {response.StatusCode}");
            return "Necunoscut";
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        var oraseSugestii = JsonSerializer.Deserialize<List<CoordonateOrasDto>>(responseBody);

        return oraseSugestii?.FirstOrDefault()?.NumeOras ?? "Necunoscut";
    }

    public async Task<CoordonateOrasDto?> ObtineCoordonateOrasExterne(string oras)
    {
        var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(oras)}";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "BookingApp/1.0 (contact@exemplu.com)");

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[EROARE] Eșec la apelul API: {response.StatusCode}");
            return null;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<List<OpenStreetMapResponse>>(jsonString);

        if (json == null || json.Count == 0)
        {
            return null; // Orașul nu a fost găsit
        }

        return new CoordonateOrasDto
        {
            NumeOras = oras,
            Latitudine = double.Parse(json[0].Latitudine),
            Longitudine = double.Parse(json[0].Longitudine)
        };
    }

}
