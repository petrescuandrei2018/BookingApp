using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repository
{
    public class RecenzieRepository : IRecenzieRepository
    {
        private readonly AppDbContext _context;

        public RecenzieRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recenzie>> ObtineRecenziiHotel(int hotelId)
        {
            var recenzii = await _context.Recenzii
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

            // Debugging: Verificăm tipul coloanei Rating
            foreach (var recenzie in recenzii)
            {
                Console.WriteLine($"Tipul Rating: {recenzie.Rating.GetType()} - Valoare: {recenzie.Rating}");
            }

            return recenzii;
        }

        public async Task<Recenzie?> ObtineRecenzieUtilizator(int utilizatorId, int hotelId)
        {
            return await _context.Recenzii
                .FirstOrDefaultAsync(r => r.UtilizatorId == utilizatorId && r.HotelId == hotelId);
        }

        public async Task AdaugaRecenzie(Recenzie recenzie)
        {
            await _context.Recenzii.AddAsync(recenzie);
            await _context.SaveChangesAsync();
        }

        public async Task StergeRecenzie(int recenzieId)
        {
            var recenzie = await _context.Recenzii.FindAsync(recenzieId);
            if (recenzie != null)
            {
                _context.Recenzii.Remove(recenzie);
                await _context.SaveChangesAsync();
            }
        }
    }
}
