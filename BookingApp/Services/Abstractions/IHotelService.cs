using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IHotelService
    {
        Task<ResponseHotelDto> GetAllHotels();
    }
}
