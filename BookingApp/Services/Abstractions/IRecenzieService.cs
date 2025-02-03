using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IRecenzieService
    {
        Task<List<RecenzieDto>> ObtineRecenziiHotel(int hotelId);
        Task AdaugaRecenzie(RecenzieDto recenzieDto);
        Task StergeRecenzie(int recenzieId);
    }
}
