using AutoMapper;
using BookingApp.Data;
using BookingApp.Helpers;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _database;
        private readonly IServiciuCacheRedis _serviciuCacheRedis;

        public HotelRepository(AppDbContext database, IMapper mapper, IServiciuCacheRedis serviciuCacheRedis)
        {
            _database = database;
            _serviciuCacheRedis = serviciuCacheRedis;
        }

        public async Task<List<Hotel>> GetAllHotels()
        {
            return await _database.Hotels.ToListAsync();
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          select new HotelTipCamera
                          {
                              HotelName = hotel.Name,
                              Address = hotel.Address,
                              TipCameraName = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              TipCameraId = tipCamera.TipCameraId
                          }).ToListAsync();
        }


        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          join pretCamera in _database.PretCamere on tipCamera.TipCameraId equals pretCamera.TipCameraId
                          select new HotelTipCameraPret
                          {
                              HotelName = hotel.Name,
                              Address = hotel.Address,
                              TipCameraName = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              PretNoapte = pretCamera.PretNoapte,
                              StartPretCamera = pretCamera.StartPretCamera,
                              EndPretCamera = pretCamera.EndPretCamera
                          }).ToListAsync();
        }


        public async Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating()
        {
            return await (from hotel in _database.Hotels
                          join tipCamera in _database.TipCamere on hotel.HotelId equals tipCamera.HotelId
                          join pretCamera in _database.PretCamere on tipCamera.TipCameraId equals pretCamera.TipCameraId
                          join review in _database.Reviews on hotel.HotelId equals review.HotelId
                          select new HotelTipCameraPretReview
                          {
                              HotelName = hotel.Name,
                              Address = hotel.Address,
                              TipCameraName = tipCamera.Name,
                              CapacitatePersoane = tipCamera.CapacitatePersoane,
                              NrTotalCamere = tipCamera.NrTotalCamere,
                              NrCamereDisponibile = tipCamera.NrCamereDisponibile,
                              NrCamereOcupate = tipCamera.NrCamereOcupate,
                              PretNoapte = pretCamera.PretNoapte,
                              StartPretCamera = pretCamera.StartPretCamera,
                              EndPretCamera = pretCamera.EndPretCamera,
                              Rating = review.Rating,
                              Description = review.Description
                          }).ToListAsync();
        }


        public async Task<List<Review>> GetAllReviews()
        {
            return await _database.Reviews.ToListAsync();
        }

        public async Task<IEnumerable<Rezervare>> GetNonExpiredRezervari()
        {
            return await _database.Rezervari.Where(r => r.CheckOut > DateTime.UtcNow).ToListAsync();
        }

        public async Task<List<PretCamera>> GetAllPretCamere()
        {
            return await _database.PretCamere.ToListAsync();
        }

        public async Task<List<Rezervare>> GetAllRezervariAsync()
        {
            return await _database.Rezervari.ToListAsync();
        }

        public async Task<List<TipCamera>> GetAllTipCamereAsync()
        {
            return await _database.TipCamere.ToListAsync();
        }

        public async Task AdaugaRezervare(Rezervare rezervare, int tipCameraId)
        {
            // Adaugăm rezervarea
            _database.Rezervari.Add(rezervare);

            // Actualizăm disponibilitatea camerei
            var tipCamera = await _database.TipCamere.FirstAsync(tc => tc.TipCameraId == tipCameraId);
            tipCamera.NrCamereDisponibile--;
            tipCamera.NrCamereOcupate++;

            await _database.SaveChangesAsync();
        }


        public async Task<Rezervare> GetRezervareByIdAsync(int rezervareId)
        {
            return await _database.Rezervari
                .Include(r => r.User)
                .Include(r => r.PretCamera)
                .FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
        }

        public async Task ActualizeazaRezervareAsync(Rezervare rezervare)
        {
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaPreturiRezervareAsync(Rezervare rezervare)
        {
            rezervare.CalculareSumaTotala();
            rezervare.SumaRamasaDePlata = rezervare.SumaTotala;

            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaStarePlataAsync(int rezervareId, string starePlata)
        {
            var rezervare = await GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            rezervare.StarePlata = starePlata;
            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task ActualizeazaPlataPartialaAsync(int rezervareId, decimal sumaAchitata)
        {
            var rezervare = await GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            rezervare.SumaAchitata += sumaAchitata;
            rezervare.SumaRamasaDePlata = rezervare.SumaTotala - rezervare.SumaAchitata;

            if (rezervare.SumaRamasaDePlata <= 0)
            {
                rezervare.StarePlata = "Platita";
            }

            _database.Rezervari.Update(rezervare);
            await _database.SaveChangesAsync();
        }

        public async Task<Rezervare> GetRezervareByPaymentIntentAsync(string paymentIntentId)
        {
            return await _database.Rezervari.FirstOrDefaultAsync(r => r.ClientSecret == paymentIntentId);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _database.Users.ToListAsync();
        }
        public async Task<PretCamera?> GetPretCameraById(int pretCameraId)
        {
            return await _database.PretCamere
                .Include(pc => pc.TipCamera)
                .ThenInclude(tc => tc.Hotel)
                .FirstOrDefaultAsync(pc => pc.PretCameraId == pretCameraId);
        }

        public async Task<int> GetDisponibilitateCamera(int pretCameraId)
        {
            var tipCamera = await _database.PretCamere
                .Where(pc => pc.PretCameraId == pretCameraId)
                .Select(pc => pc.TipCamera)
                .FirstOrDefaultAsync();

            return tipCamera?.NrCamereDisponibile ?? 0;
        }

    }
}
