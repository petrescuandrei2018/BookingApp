using BookingApp.Models.Dtos;
using BookingApp.Models;

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
        Task<List<GetAllRezervariDto>> GetAllRezervariAsync(); // Actualizat tipul de returnare
        Task<RezervareDto> CreateRezervareFromDto(RezervareDto rezervareDto);
        Task<IEnumerable<GetAllRezervariDto>> GetNonExpiredRezervari();
        Task ProcesarePlataStripeAsync(int rezervareId);
        Task<Rezervare> GetRezervareByIdAsync(int rezervareId);
        Task ActualizeazaRezervareAsync(Rezervare rezervare);
        Task ProcesarePlataPartialaAsync(int rezervareId, decimal sumaAchitata);
        Task RefundPaymentAsync(string paymentIntentId, decimal? suma);
        Task<List<RezervareRefundDto>> GetRezervariEligibileRefund();
        Task<List<object>> GetRezervariEligibilePlata(); // Nouă metodă pentru rezervările eligibile de plată
        Task<decimal> ObțineSumaAchitatăAsync(int rezervareId);
        Task<List<HotelCuDistantaDto>> ObtineHoteluriPeLocatie(double latitudine, double longitudine, double razaKm);
        Task<List<HotelCuDistantaDto>> ObtineHoteluriPeOras(string oras, double razaKm);
        Task<string> GenereazaHartaAsync(string oras, double razaKm);

        Task<string> GenereazaHartaHtmlAsync(string oras, double razaKm);

        Task<CoordonateOrasDto?> ObțineCoordonateOras(string oras);

        Task<List<HotelCoordonateDto>> GetAllHotelsCoordonate(); // <- Noua metodă

    }
}
