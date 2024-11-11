using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _database;

        public HotelRepository(AppDbContext database)
        {
            _database = database;
        }
        public async Task<List<Hotel>> GetAllHotels()
        {
            var listHotels = _database.Hotels.ToList();
            return listHotels;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere =   (from hotels in _database.Hotels
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
                                        NrCamereOcupate = tipCamere.NrCamereOcupate
                                    }).ToList();
             return hotelsTipCamere;
        }
    }
}
