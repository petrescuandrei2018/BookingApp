using BookingApp.Models;
using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IHotelService
    {
        Task<List<ResponseHotelDto>> GetAllHotels();
        Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume);
        Task<List<HotelTipCamera>> GetAllHotelsTipCamera();
        Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane);
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret();
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane,
            float? pret);
    }
}
