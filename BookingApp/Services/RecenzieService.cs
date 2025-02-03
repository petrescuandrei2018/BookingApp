using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using AutoMapper;

namespace BookingApp.Services
{
    public class RecenzieService : IRecenzieService
    {
        private readonly IRecenzieRepository _recenzieRepository;
        private readonly IMapper _mapper;

        public RecenzieService(IRecenzieRepository recenzieRepository, IMapper mapper)
        {
            _recenzieRepository = recenzieRepository;
            _mapper = mapper;
        }

        public async Task<List<RecenzieDto>> ObtineRecenziiHotel(int hotelId)
        {
            var recenzii = await _recenzieRepository.ObtineRecenziiHotel(hotelId);
            return _mapper.Map<List<RecenzieDto>>(recenzii);
        }

        public async Task AdaugaRecenzie(RecenzieDto recenzieDto)
        {
            var recenzie = _mapper.Map<Recenzie>(recenzieDto);
            await _recenzieRepository.AdaugaRecenzie(recenzie);
        }

        public async Task StergeRecenzie(int recenzieId)
        {
            await _recenzieRepository.StergeRecenzie(recenzieId);
        }
    }
}
