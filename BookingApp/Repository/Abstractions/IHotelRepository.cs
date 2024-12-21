using BookingApp.Migrations;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using PretCamera = BookingApp.Models.PretCamera;

namespace BookingApp.Repository.Abstractions
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotels();
        Task<List<HotelTipCamera>> GetAllHotelsTipCamera();
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret();
        Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating();
        Task<List<Review>> GetAllReviews();
        Task<List<PretCamera>> GetAllPretCamere();
        Task/*<RezervareDto>*/ AdaugaRezervare(Rezervare rezervare, int tipCameraId);
        Task<List<User>> GetAllUsers();
    }
}
