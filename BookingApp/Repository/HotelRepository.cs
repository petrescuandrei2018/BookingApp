using AutoMapper;
using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    // Repository pentru gestionarea datelor despre hoteluri
    public class HotelRepository : IHotelRepository
    {
        // Contextul bazei de date
        private readonly AppDbContext _database;

        // Constructor pentru injectarea dependențelor
        public HotelRepository(AppDbContext database, IMapper mapper)
        {
            _database = database;
        }

        // Metodă pentru a actualiza o rezervare
        public async Task ActualizeazaRezervareAsync(Rezervare rezervare)
        {
            _database.Rezervari.Update(rezervare); // Marchează rezervarea pentru actualizare
            await _database.SaveChangesAsync(); // Salvează schimbările în baza de date
        }


        // Metodă pentru adăugarea unei rezervări și actualizarea disponibilității camerelor
        public async Task/*<RezervareDto>*/ AdaugaRezervare(Rezervare rezervare, int tipCameraId)
        {
            _database.Rezervari.Add(rezervare);

            // Actualizarea disponibilității camerelor
            var tipCameraUpdated = (from tipCamera in _database.TipCamere
                                    where tipCamera.TipCameraId == tipCameraId
                                    select tipCamera).First();
            tipCameraUpdated.NrCamereDisponibile--;
            tipCameraUpdated.NrCamereOcupate++;
            _database.SaveChanges();

            RezervareDto rezervareDto = new RezervareDto
            {
                PretCameraId = rezervare.PretCameraId,
                UserId = rezervare.UserId,
                CheckIn = rezervare.CheckIn,
                CheckOut = rezervare.CheckOut
            };
        }

        // Obține toate hotelurile
        public async Task<List<Hotel>> GetAllHotels()
        {
            var listHotels = _database.Hotels.ToList();
            return listHotels;
        }

        // Obține hotelurile și recenziile, filtrate după rating
        public async Task<List<HotelTipCameraPretReview>> GetAllHotelsByRating()
        {
            var hotelsTipCamerePretReview = (from hotels in _database.Hotels
                                             join tipCamere in _database.TipCamere
                                             on hotels.HotelId equals tipCamere.HotelId
                                             join pretCamere in _database.PretCamere
                                             on tipCamere.TipCameraId equals pretCamere.TipCameraId
                                             join reviews in _database.Reviews
                                             on hotels.HotelId equals reviews.HotelId
                                             select new HotelTipCameraPretReview
                                             {
                                                 HotelName = hotels.Name,
                                                 Address = hotels.Address,
                                                 Rating = reviews.Rating,
                                                 Description = reviews.Description
                                             }).ToList();

            return hotelsTipCamerePretReview;
        }

        // Obține hotelurile împreună cu tipurile de camere
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere = (from hotels in _database.Hotels
                                   join tipCamere in _database.TipCamere
                                   on hotels.HotelId equals tipCamere.HotelId
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
                                   }).ToList();
            return hotelsTipCamere;
        }

        // Obține hotelurile împreună cu tipurile de camere și prețurile
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            var hotelsTipCamerePret = (from hotels in _database.Hotels
                                       join tipCamere in _database.TipCamere
                                       on hotels.HotelId equals tipCamere.HotelId
                                       join pretCamere in _database.PretCamere
                                       on tipCamere.TipCameraId equals pretCamere.TipCameraId
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
                                       }).ToList();

            return hotelsTipCamerePret;
        }

        // Obține lista de prețuri pentru camere
        public async Task<List<PretCamera>> GetAllPretCamere()
        {
            var pretCamere = _database.PretCamere.ToList();
            return pretCamere;
        }

        // Obține toate recenziile
        public async Task<List<Review>> GetAllReviews()
        {
            var reviews = _database.Reviews.ToList();
            return reviews;
        }

        // Obține toate rezervările, inclusiv informațiile despre camere
        public async Task<List<Rezervare>> GetAllRezervariAsync()
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera)
                .ToListAsync();
        }

        // Obține toate tipurile de camere
        public async Task<List<TipCamera>> GetAllTipCamereAsync()
        {
            return await _database.TipCamere.ToListAsync();
        }

        // Obține lista cu toți utilizatorii
        public async Task<List<User>> GetAllUsers()
        {
            var users = _database.Users.ToList();
            return users;
        }

        // Metodă pentru a obține o rezervare după ID
        public async Task<Rezervare> GetRezervareByIdAsync(int rezervareId)
        {
            return await _database.Rezervari
                .Include(r => r.PretCamera) // Încarcă și entitatea PretCamera asociată
                .FirstOrDefaultAsync(r => r.RezervareId == rezervareId);
        }
    }
}
