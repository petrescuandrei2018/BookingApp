using BookingApp.Data;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Services
{
    public class StatisticiService : IStatisticiService
    {
        private readonly AppDbContext _context;

        public StatisticiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatisticiDto> ObtineStatisticiDashboardAsync()
        {
            var rezervariActive = await GetNumarRezervariActiveAsync();
            var venituri = await GetVenituriAsync();
            var hoteluriFrecvente = await GetCeleMaiRezervateHoteluriAsync();
            var utilizatoriStatistici = await GetUtilizatoriStatisticiAsync();

            return new DashboardStatisticiDto
            {
                RezervariActive = rezervariActive,
                VenitZi = venituri["zi"],
                VenitSaptamana = venituri["saptamana"],
                VenitLuna = venituri["luna"],
                HoteluriFrecvente = hoteluriFrecvente,
                UtilizatoriActivi = utilizatoriStatistici.UtilizatoriActivi,
                UtilizatoriInactivi = utilizatoriStatistici.UtilizatoriInactivi
            };
        }

        public async Task<int> GetNumarRezervariActiveAsync()
        {
            var azi = DateTime.UtcNow.Date;
            return await _context.Rezervari.CountAsync(r => r.CheckOut > azi);
        }

        public async Task<Dictionary<string, decimal>> GetVenituriAsync()
        {
            var azi = DateTime.UtcNow.Date;
            var inceputSaptamana = azi.AddDays(-(int)azi.DayOfWeek);
            var inceputLuna = new DateTime(azi.Year, azi.Month, 1);

            return new Dictionary<string, decimal>
            {
                { "zi", await _context.Rezervari.Where(r => r.CheckIn.Date == azi).SumAsync(r => r.SumaAchitata) },
                { "saptamana", await _context.Rezervari.Where(r => r.CheckIn >= inceputSaptamana).SumAsync(r => r.SumaAchitata) },
                { "luna", await _context.Rezervari.Where(r => r.CheckIn >= inceputLuna).SumAsync(r => r.SumaAchitata) }
            };
        }

        public async Task<List<HotelRezervariDto>> GetCeleMaiRezervateHoteluriAsync()
        {
            var hoteluriFrecvente = await _context.Rezervari
                .GroupBy(r => r.PretCamera.TipCamera.Hotel.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => new HotelRezervariDto
                {
                    NumeHotel = g.Key,
                    NumarRezervari = g.Count()
                })
                .Take(5)
                .ToListAsync();

            return hoteluriFrecvente;
        }

        public async Task<UtilizatoriStatisticiDto> GetUtilizatoriStatisticiAsync()
        {
            var utilizatoriActivi = await _context.Users.CountAsync(u => _context.Rezervari.Any(r => r.UserId == u.UserId));
            var utilizatoriInactivi = await _context.Users.CountAsync() - utilizatoriActivi;

            return new UtilizatoriStatisticiDto
            {
                UtilizatoriActivi = utilizatoriActivi,
                UtilizatoriInactivi = utilizatoriInactivi
            };
        }
    }
}
