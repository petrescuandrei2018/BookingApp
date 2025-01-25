using BookingApp.Models;
using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IHotelService
    {
        Task<List<ResponseHotelDto>> GetAllHotels();
        Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume);
        Task<List<HotelWithRating>> GetAllHotelsByRating(double? rating);
        Task<List<HotelTipCamera>> GetAllHotelsTipCamera();
        Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane);
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret();
        Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPretFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pretCamera);
        Task<List<GetAllRezervariDto>> GetAllRezervariAsync();
        Task<RezervareDto> CreateRezervareFromDto(RezervareDto rezervareDto);
        Task<IEnumerable<GetAllRezervariDto>> GetNonExpiredRezervari();
        Task ProcesarePlataStripeAsync(int rezervareId);
        Task<Rezervare> GetRezervareByIdAsync(int rezervareId);
        Task ActualizeazaRezervareAsync(Rezervare rezervare);
        Task ProcesarePlataPartialaAsync(int rezervareId, decimal sumaAchitata);
        Task RefundPaymentAsync(string paymentIntentId, decimal? suma);
        Task<List<object>> GetRezervariEligibileRefund();

    }
}
