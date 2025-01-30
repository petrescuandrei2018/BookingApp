using System.Threading.Tasks;
using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuCoordonate
    {
        Task<CoordonateOrasDto?> ObtineCoordonateOras(string oras);
    }
}
