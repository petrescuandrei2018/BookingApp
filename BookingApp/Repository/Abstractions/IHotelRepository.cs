using BookingApp.Models;

namespace BookingApp.Repository.Abstractions
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotels();
        Task<List<HotelTipCamera>> GetAllHotelsTipCamera();
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret();
    }
}
