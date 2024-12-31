using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<RezervareDto> CreateRezervareFromDto(RezervareDto rezervareDto)
        {
            // Validare: CheckOut nu poate fi mai mic decât CheckIn
            if (rezervareDto.CheckOut < rezervareDto.CheckIn)
            {
                throw new Exception("Data de CheckOut nu poate fi mai mică decât data de CheckIn.");
            }

            // Verificăm dacă utilizatorul există
            var users = await _hotelRepository.GetAllUsers();
            var validUser = users.Any(x => x.UserId == rezervareDto.UserId);
            if (!validUser)
            {
                throw new Exception($"Utilizatorul cu ID {rezervareDto.UserId} nu există.");
            }

            // Verificăm dacă prețul camerei este valid
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var validPretCamera = pretCamere.FirstOrDefault(x => x.PretCameraId == rezervareDto.PretCameraId);
            if (validPretCamera == null)
            {
                throw new Exception($"Camera cu ID {rezervareDto.PretCameraId} nu există.");
            }

            // Verificăm disponibilitatea camerelor
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            var rezervareTipCameraId = validPretCamera.TipCameraId;

            var nrCamereDisponibile = hotelsTipCamere
                .Where(htc => htc.TipCameraId == rezervareTipCameraId)
                .Select(htc => htc.NrCamereDisponibile)
                .FirstOrDefault();

            if (nrCamereDisponibile < 1)
            {
                throw new Exception($"Nu există camere disponibile pentru TipCameraId {rezervareTipCameraId}.");
            }

            // Creăm rezervarea
            var rezervare = _mapper.Map<Rezervare>(rezervareDto);

            // Stabilim Starea în funcție de datele CheckIn și CheckOut
            if (rezervareDto.CheckOut < DateTime.UtcNow)
            {
                rezervare.Stare = StareRezervare.Expirata;
            }
            else if (rezervareDto.CheckIn <= DateTime.UtcNow && rezervareDto.CheckOut >= DateTime.UtcNow)
            {
                rezervare.Stare = StareRezervare.Activa;
            }
            else
            {
                rezervare.Stare = StareRezervare.Viitoare;
            }

            // Adăugăm rezervarea în baza de date
            await _hotelRepository.AdaugaRezervare(rezervare, rezervareTipCameraId);

            return rezervareDto;
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

        public async Task<List<GetAllRezervariDto>> GetAllRezervariAsync()
        {
            // Pasul 1: Preluarea datelor din repository
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hotels = await _hotelRepository.GetAllHotels();
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var tipCamere = await _hotelRepository.GetAllTipCamereAsync(); // Preluarea tipurilor de camere

            // Pasul 2: Combinarea datelor pentru a construi DTO-uri
            var rezervariDto = new List<GetAllRezervariDto>();

            foreach (var rezervare in rezervari)
            {
                // Găsirea camerei aferente rezervării
                var camera = pretCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                if (camera == null)
                {
                    throw new Exception($"Nu a fost găsită o cameră corespunzătoare pentru RezervareId: {rezervare.RezervareId}");
                }

                // Găsirea tipului de cameră aferent camerei
                var tipCamera = tipCamere.FirstOrDefault(tc => tc.TipCameraId == camera.TipCameraId);
                if (tipCamera == null)
                {
                    throw new Exception($"Nu a fost găsit un tip de cameră corespunzător pentru PretCameraId: {camera.PretCameraId}");
                }

                // Găsirea hotelului aferent tipului de cameră
                var hotel = hotels.FirstOrDefault(h => h.HotelId == tipCamera.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Nu a fost găsit un hotel corespunzător pentru TipCameraId: {tipCamera.TipCameraId}");
                }

                // Construirea obiectului DTO
                var rezervareDto = new GetAllRezervariDto
                {
                    UserId = rezervare.UserId, // Atribuirea UserId din entitatea Rezervare
                    HotelName = hotel.Name, // Numele hotelului
                    CheckIn = rezervare.CheckIn, // Data de check-in
                    CheckOut = rezervare.CheckOut, // Data de check-out
                    Pret = (decimal)camera.PretNoapte, // Prețul camerei pe noapte, convertit în decimal
                    Stare = rezervare.Stare.ToString() // Enum-ul StareRezervare
                };

                rezervariDto.Add(rezervareDto);
            }

            // Returnarea listei de DTO-uri
            return rezervariDto;
        }

        public async Task<List<GetAllRezervariDto>> GetNonExpiredRezervariAsync()
        {
            // Pasul 1: Preluăm datele din repository
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hoteluri = await _hotelRepository.GetAllHotels();
            var preturiCamere = await _hotelRepository.GetAllPretCamere();
            var tipuriCamere = await _hotelRepository.GetAllTipCamereAsync();

            // Pasul 2: Filtrăm rezervările care nu sunt expirate
            var rezervariDto = new List<GetAllRezervariDto>();

            foreach (var rezervare in rezervari.Where(r => r.Stare != StareRezervare.Expirata))
            {
                // Găsim camera asociată rezervării
                var camera = preturiCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                if (camera == null)
                {
                    throw new Exception($"Nu a fost găsită o cameră asociată pentru RezervareId: {rezervare.RezervareId}");
                }

                // Găsim tipul de cameră asociat
                var tipCamera = tipuriCamere.FirstOrDefault(tc => tc.TipCameraId == camera.TipCameraId);
                if (tipCamera == null)
                {
                    throw new Exception($"Nu a fost găsit un TipCamera asociat pentru PretCameraId: {camera.PretCameraId}");
                }

                // Găsim hotelul asociat
                var hotel = hoteluri.FirstOrDefault(h => h.HotelId == tipCamera.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Nu a fost găsit un Hotel asociat pentru TipCameraId: {tipCamera.TipCameraId}");
                }

                // Construim obiectul DTO
                var rezervareDto = new GetAllRezervariDto
                {
                    UserId = rezervare.UserId, // ID-ul utilizatorului
                    HotelName = hotel.Name, // Numele hotelului
                    CheckIn = rezervare.CheckIn, // Data de check-in
                    CheckOut = rezervare.CheckOut, // Data de check-out
                    Pret = (decimal)camera.PretNoapte, // Prețul camerei
                    Stare = rezervare.Stare.ToString() // Starea rezervării
                };

                rezervariDto.Add(rezervareDto);
            }

            // Returnăm lista DTO-urilor filtrate
            return rezervariDto;
        }
    }
}
