// IHotelRepository.cs
using BookingApp.Models;
using BookingApp.Models.Dtos;

namespace BookingApp.Repository.Abstractions
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetAllHotels();
        Task<List<HotelTipCamera>> GetAllHotelsTipCamera();
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret();
        Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating();
        Task<List<Review>> GetAllReviews();
        Task<IEnumerable<Rezervare>> GetNonExpiredRezervari();
        Task<List<PretCamera>> GetAllPretCamere();
        Task<List<Rezervare>> GetAllRezervariAsync();
        Task<List<TipCamera>> GetAllTipCamereAsync();
        Task AddRezervareAsync(Rezervare rezervare);
        Task<List<User>> GetAllUsers();
        Task<Rezervare> GetRezervareByIdAsync(int rezervareId);
        Task ActualizeazaRezervareAsync(Rezervare rezervare);
        Task ActualizeazaPreturiRezervareAsync(Rezervare rezervare);
        Task ActualizeazaStarePlataAsync(int rezervareId, string starePlata);
        Task ActualizeazaPlataPartialaAsync(int rezervareId, decimal sumaAchitata);
        Task<Rezervare> GetRezervareByPaymentIntentAsync(string paymentIntentId);

        Task<PretCamera?> GetPretCameraById(int pretCameraId);
        Task<int> GetDisponibilitateCamera(int pretCameraId);

        Task<TipCamera> GetTipCameraByNameAsync(string numeCamera);
        Task<PretCamera> GetPretCameraByTipCameraIdAsync(int tipCameraId);
        Task<Hotel?> GetHotelByNameAsync(string hotelName);

    }

}
