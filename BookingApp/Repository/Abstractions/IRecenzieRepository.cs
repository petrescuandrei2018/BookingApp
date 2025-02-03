using BookingApp.Models;

namespace BookingApp.Repository.Abstractions
{
    public interface IRecenzieRepository
    {
        Task<List<Recenzie>> ObtineRecenziiHotel(int hotelId);
        Task<Recenzie?> ObtineRecenzieUtilizator(int utilizatorId, int hotelId);
        Task AdaugaRecenzie(Recenzie recenzie);
        Task StergeRecenzie(int recenzieId);
    }
}
