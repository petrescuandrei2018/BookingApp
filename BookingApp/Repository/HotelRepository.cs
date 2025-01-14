// HotelRepository.cs
using AutoMapper;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _database;

        public HotelRepository(AppDbContext database, IMapper mapper)
        {
            _database = database;
        }

        public async Task ActualizeazaPreturiRezervareAsync(Rezervare rezervare)
        {
            rezervare.CalculareSumaTotala();
            rezervare.SumaRamasaDePlata = rezervare.SumaTotala;
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaRezervareAsync(Rezervare rezervare)
        {
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task AdaugaRezervare(Rezervare rezervare, int tipCameraId)
        {
            _database.Rezervari.Add(rezervare);

            var tipCameraUpdated = await _database.TipCamere.FirstAsync(tc => tc.TipCameraId == tipCameraId);
            tipCameraUpdated.NrCamereDisponibile--;
            tipCameraUpdated.NrCamereOcupate++;

            await _database.SaveChangesAsync();
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            return await _database.Hotels.ToListAsync();
        }

        public async Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating()
        {
            return await (from hotels in _database.Hotels
                          join tipCamere in _database.TipCamere on hotels.HotelId equals tipCamere.HotelId
                          join pretCamere in _database.PretCamere on tipCamere.TipCameraId equals pretCamere.TipCameraId
                          join reviews in _database.Reviews on hotels.HotelId equals reviews.HotelId
                          select new HotelTipCameraPretReview
                          {
                              HotelName = hotels.Name,
                              Address = hotels.Address,
                              Rating = reviews.Rating,
                              Description = reviews.Description
                          }).ToListAsync();
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            return await (from hotels in _database.Hotels
                          join tipCamere in _database.TipCamere on hotels.HotelId equals tipCamere.HotelId
                          select new HotelTipCamera
                          {
                              HotelName = hotels.Name,
                              Address = hotels.Address,
                              TipCameraName = tipCamere.Name,
                              CapacitatePersoane = tipCamere.CapacitatePersoane,
                              NrTotalCamere = tipCamere.NrTotalCamere,
                              NrCamereDisponibile = tipCamere.NrCamereDisponibile,
                              NrCamereOcupate = tipCamere.NrCamereOcupate,
                              TipCameraId = tipCamere.TipCameraId
                          }).ToListAsync();
        }

        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            return await (from hotels in _database.Hotels
                          join tipCamere in _database.TipCamere on hotels.HotelId equals tipCamere.HotelId
                          join pretCamere in _database.PretCamere on tipCamere.TipCameraId equals pretCamere.TipCameraId
                          select new HotelTipCameraPret
                          {
                              HotelName = hotels.Name,
                              Address = hotels.Address,
                              TipCameraName = tipCamere.Name,
                              CapacitatePersoane = tipCamere.CapacitatePersoane,
                              NrTotalCamere = tipCamere.NrTotalCamere,
                              NrCamereDisponibile = tipCamere.NrCamereDisponibile,
                              NrCamereOcupate = tipCamere.NrCamereOcupate,
                              PretNoapte = pretCamere.PretNoapte,
                              StartPretCamera = pretCamere.StartPretCamera,
                              EndPretCamera = pretCamere.EndPretCamera
                          }).ToListAsync();
        }

        public async Task<List<PretCamera>> GetAllPretCamere()
        {
            return await _database.PretCamere.ToListAsync();
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _database.Reviews.ToListAsync();
        }

        public async Task<List<Rezervare>> GetAllRezervariAsync()
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera)
                .ToListAsync();
        }

        public async Task<List<TipCamera>> GetAllTipCamereAsync()
        {
            return await _database.TipCamere.ToListAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _database.Users.ToListAsync();
        }

        public async Task<Rezervare> GetRezervareByIdAsync(int rezervareId)
        {
            return await _database.Rezervari
                .Include(r => r.User)
                .Include(r => r.PretCamera)
                .FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
        }
    }
}
