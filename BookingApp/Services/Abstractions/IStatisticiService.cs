using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IStatisticiService
    {
        Task<int> GetNumarRezervariActiveAsync();
        Task<Dictionary<string, decimal>> GetVenituriAsync();
        Task<List<HotelRezervariDto>> GetCeleMaiRezervateHoteluriAsync();
        Task<UtilizatoriStatisticiDto> GetUtilizatoriStatisticiAsync();
        Task<DashboardStatisticiDto> ObtineStatisticiDashboardAsync(); // 🔹 Adăugat
    }
}
