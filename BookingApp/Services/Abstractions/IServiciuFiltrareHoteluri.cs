using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuFiltrareHoteluri
    {
        Task<List<HotelCoordonateDto>> FiltreazaHoteluri(string oras, double razaKm);
    }
}
