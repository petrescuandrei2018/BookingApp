using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;

namespace BookingApp.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private IMapper _mapper;
        public HotelService(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }
        public async Task<List<ResponseHotelDto>> GetAllHotels()
        {
            List<ResponseHotelDto> responseHotelDtos = new List<ResponseHotelDto>();
            var hotels = await _hotelRepository.GetAllHotels();
            responseHotelDtos = _mapper.Map<List<ResponseHotelDto>>(hotels);
            return responseHotelDtos;
        }
    }
}
