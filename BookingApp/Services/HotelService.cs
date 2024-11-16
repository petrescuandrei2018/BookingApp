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

        public async Task<List<HotelWithRating>> GetAllHotelsByRating(double? rating)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            List<HotelWithRating> hotelsWithRatings = new List<HotelWithRating>();
            foreach (var hotel in hotels)
            {
                var hotelsRating = await _hotelRepository.GetAllHotelsByRating();
                var reviews = await _hotelRepository.GetAllReviews();
                var reviewsCount = (from review in reviews
                                   where review.HotelId == hotel.HotelId
                                   select review.ReviewId).Count();
                var ratingMediu =  (from review in reviews
                                   where review.HotelId == hotel.HotelId
                                   select review.Rating).Sum();
                HotelWithRating hR1 = new HotelWithRating();
                hR1.Address = hotel.Address;
                if(reviewsCount ==  0)
                {
                    hR1.Rating = 0;
                }
                else
                {
                    hR1.Rating = ratingMediu / reviewsCount;
                }
                hR1.HotelName = hotel.Name;
                hR1.ReviewuriTotale = reviewsCount;
                if(hR1.Rating >= rating)
                {
                    hotelsWithRatings.Add(hR1);
                }
            }
            return hotelsWithRatings;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            return hotelsTipCamere;
        }

        public async Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane)
        {
            List<HotelTipCamera> hotelsTipCamere = new List<HotelTipCamera>();
            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane>0)
            {
                hotelsTipCamere =  await _hotelRepository.GetAllHotelsTipCamera();
                hotelsTipCamere = hotelsTipCamere.Where(x => x.HotelName == filtruNumeHotel).ToList();
                if (hotelsTipCamere.Count > 0)
                {
                    hotelsTipCamere = hotelsTipCamere.Where(x => x.CapacitatePersoane == capacitatePersoane).ToList();
                }
            }
            return hotelsTipCamere;
        }

        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            var hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
            return hotelsTipCamerePret;
        }

        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPretFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pretCamera)
        {
            List<HotelTipCameraPret> hotelsTipCamerePret = new List<HotelTipCameraPret>();
            if ((!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane > 0) && pretCamera > 0)
            {
                hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
                hotelsTipCamerePret = hotelsTipCamerePret.Where(x => x.HotelName == filtruNumeHotel).ToList();
                if (hotelsTipCamerePret.Count > 0)
                {
                    hotelsTipCamerePret = hotelsTipCamerePret.Where(x => x.CapacitatePersoane == capacitatePersoane).ToList();
                }
                if (hotelsTipCamerePret.Count > 0)
                {
                    hotelsTipCamerePret = hotelsTipCamerePret.Where(x => x.PretNoapte <= pretCamera).ToList();
                }
            }
            return hotelsTipCamerePret;
        }
    }
}
