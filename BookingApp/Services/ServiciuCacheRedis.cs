using BookingApp.Services.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class ServiciuCacheRedis : IServiciuCacheRedis
    {
        private readonly IDistributedCache _cacheRedis;

        public ServiciuCacheRedis(IDistributedCache cacheRedis)
        {
            _cacheRedis = cacheRedis;
        }

        // Setează o valoare în Redis
        public async Task SeteazaValoareAsync<T>(string cheia, T valoare, TimeSpan durataExpirare)
        {
            var dateSerializate = JsonSerializer.Serialize(valoare);
            await _cacheRedis.SetStringAsync(cheia, dateSerializate, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = durataExpirare
            });
        }

        // Obține o valoare din Redis pe baza cheii
        public async Task<T> ObtineValoareAsync<T>(string cheia)
        {
            var dateSerializate = await _cacheRedis.GetStringAsync(cheia);
            if (dateSerializate == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(dateSerializate);
        }

        // Șterge o valoare din Redis pe baza cheii
        public async Task StergeValoareAsync(string cheia)
        {
            await _cacheRedis.RemoveAsync(cheia);
        }
    }
}
