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
    }
}
