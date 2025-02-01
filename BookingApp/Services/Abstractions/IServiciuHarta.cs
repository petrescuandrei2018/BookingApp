using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Services.Abstractions
{
    public interface IServiciuHarta
    {
        Task<FileResult> GenereazaSiSalveazaHarta(string oras, double razaKm);
    }
}
