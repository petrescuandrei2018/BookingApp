using System.Threading.Tasks;
using BookingApp.Models.Dtos;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuCoordonateOnline
    {
        Task<CoordonateOrasDto?> ObtineCoordonateOras(string oras);
    }
}
