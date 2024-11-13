using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using System.Linq;

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
            var hotels = await _hotelRepository.GetAllHotels();
            var hotelsResponse = _mapper.Map<List<ResponseHotelDto>>(hotels);
            return hotelsResponse;
        }

        public async Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume)
        {
            List<ResponseHotelDto> responseHotelDtos = new List<ResponseHotelDto>();
            var hotels = await _hotelRepository.GetAllHotels();

            if (!string.IsNullOrEmpty(filtruNume))
            {
                hotels = hotels.Where(x => x.Name == filtruNume).ToList();
            }


            /*if (filtruNume != null && filtruNume != string.Empty)
            {
                var hotelGasit = from hotel in hotels
                           where hotel.Name == filtruNume
                           select hotel;
                
                if(hotelGasit!=null)
                {
                    var hotelDinLista = hotelGasit.FirstOrDefault();
                    var hotelDto = _mapper.Map<ResponseHotelDto>(hotelDinLista);
                    List<ResponseHotelDto> listHotelGasit = new List<ResponseHotelDto>();
                    listHotelGasit.Add(hotelDto);
                    return listHotelGasit;
                }
            }*/

            if (hotels.Any())
            {
                responseHotelDtos = _mapper.Map<List<ResponseHotelDto>>(hotels);
            }


            return responseHotelDtos;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            return hotelsTipCamere;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane)
        {
            List<HotelTipCamera> hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            if (!string.IsNullOrEmpty(filtruNumeHotel))
            {
                hotelsTipCamere = hotelsTipCamere.Where(x => x.HotelName == filtruNumeHotel).ToList();
                if (hotelsTipCamere.Count == 0 && capacitatePersoane > 0)
                {
                    hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
                    hotelsTipCamere = hotelsTipCamere.Where(x => x.CapacitatePersoane == capacitatePersoane).ToList();
                }

                if (capacitatePersoane > 0)
                {
                    hotelsTipCamere = hotelsTipCamere.Where(x => x.CapacitatePersoane == capacitatePersoane).ToList();
                }
            }
            return hotelsTipCamere;
        }

        public Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pret)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            var hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
            return hotelsTipCamerePret;
        }

        
    }
}
