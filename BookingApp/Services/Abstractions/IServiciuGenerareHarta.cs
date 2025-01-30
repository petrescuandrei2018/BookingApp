using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuGenerareHarta
    {
        Task<string> GenereazaHartaHtml(string? oras, double? razaKm);
    }
}
