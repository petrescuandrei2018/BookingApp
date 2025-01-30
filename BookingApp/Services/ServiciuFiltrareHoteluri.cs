using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class ServiciuFiltrareHoteluri : IServiciuFiltrareHoteluri
    {
        private readonly IHotelService _serviciuHotel;
        private readonly IServiciuCoordonate _serviciuCoordonate;

        public ServiciuFiltrareHoteluri(IHotelService serviciuHotel, IServiciuCoordonate serviciuCoordonate)
        {
            _serviciuHotel = serviciuHotel;
            _serviciuCoordonate = serviciuCoordonate;
        }

        public async Task<List<HotelCoordonateDto>> FiltreazaHoteluri(string oras, double razaKm)
        {
            if (string.IsNullOrEmpty(oras))
            {
                return await _serviciuHotel.GetAllHotelsCoordonate();
            }

            var coordonate = await _serviciuCoordonate.ObtineCoordonateOras(oras);
            if (coordonate == null) return new List<HotelCoordonateDto>();

            var hoteluriCuDistanta = await _serviciuHotel.ObtineHoteluriPeOras(oras, razaKm);
            return hoteluriCuDistanta.Select(h => new HotelCoordonateDto
            {
                NumeHotel = h.NumeHotel,
                Adresa = h.Adresa,
                Latitudine = h.Latitudine,
                Longitudine = h.Longitudine
            }).ToList();
        }
    }
}
