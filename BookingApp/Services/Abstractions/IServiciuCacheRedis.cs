using System;
using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuCacheRedis
    {
        Task SeteazaValoareAsync<T>(string cheia, T valoare, TimeSpan durataExpirare);
        Task<T> ObtineValoareAsync<T>(string cheia);
        Task StergeValoareAsync(string cheia);
    }
}
