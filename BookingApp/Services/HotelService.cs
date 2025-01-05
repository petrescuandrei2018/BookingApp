using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace BookingApp.Services
{
    // Serviciu pentru gestionarea operațiunilor asupra hotelurilor
    public class HotelService : IHotelService
    {
        // Repository-ul utilizat pentru accesarea datelor despre hoteluri
        private readonly IHotelRepository _hotelRepository;

        // Mapper pentru maparea entităților
        private IMapper _mapper;

        // Constructor pentru injectarea dependențelor
        public HotelService(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        // Crearea unei rezervări pornind de la DTO
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

            // Stabilim starea rezervării în funcție de datele CheckIn și CheckOut
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

        // Obține toate hotelurile
        public async Task<List<ResponseHotelDto>> GetAllHotels()
        {
            var hotels = await _hotelRepository.GetAllHotels();
            var hotelsResponse = _mapper.Map<List<ResponseHotelDto>>(hotels);
            return hotelsResponse;
        }

        // Obține toate hotelurile filtrate după nume
        public async Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume)
        {
            List<ResponseHotelDto> responseHotelDtos = new List<ResponseHotelDto>();
            var hotels = await _hotelRepository.GetAllHotels();

            if (!string.IsNullOrEmpty(filtruNume))
            {
                hotels = hotels.Where(x => x.Name == filtruNume).ToList();
            }

            if (hotels.Any())
            {
                responseHotelDtos = _mapper.Map<List<ResponseHotelDto>>(hotels);
            }

            return responseHotelDtos;
        }

        // Obține toate hotelurile împreună cu ratingul lor
        public async Task<List<HotelWithRating>> GetAllHotelsByRating(double? rating)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            List<HotelWithRating> hotelsWithRatings = new List<HotelWithRating>();

            foreach (var hotel in hotels)
            {
                var reviews = await _hotelRepository.GetAllReviews();
                var reviewsCount = reviews.Count(r => r.HotelId == hotel.HotelId);
                var ratingMediu = reviews.Where(r => r.HotelId == hotel.HotelId).Sum(r => r.Rating);

                HotelWithRating hotelWithRating = new HotelWithRating
                {
                    Address = hotel.Address,
                    HotelName = hotel.Name,
                    ReviewuriTotale = reviewsCount,
                    Rating = reviewsCount == 0 ? 0 : ratingMediu / reviewsCount
                };

                if (hotelWithRating.Rating >= rating)
                {
                    hotelsWithRatings.Add(hotelWithRating);
                }
            }
            return hotelsWithRatings;
        }

        // Obține toate hotelurile împreună cu tipurile de camere
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            return hotelsTipCamere;
        }

        // Filtrează hotelurile după nume și capacitate persoane
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane)
        {
            var hotelsTipCamere = new List<HotelTipCamera>();

            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane > 0)
            {
                hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
                hotelsTipCamere = hotelsTipCamere.Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane).ToList();
            }
            return hotelsTipCamere;
        }

        // Obține toate hotelurile cu prețurile tipurilor de camere
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            var hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
            return hotelsTipCamerePret;
        }

        // Filtrează hotelurile după nume, capacitate persoane și prețul camerei
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPretFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pretCamera)
        {
            var hotelsTipCamerePret = new List<HotelTipCameraPret>();

            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane > 0 && pretCamera > 0)
            {
                hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
                hotelsTipCamerePret = hotelsTipCamerePret
                    .Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane && x.PretNoapte <= pretCamera)
                    .ToList();
            }

            return hotelsTipCamerePret;
        }

        // Obține toate rezervările
        public async Task<List<GetAllRezervariDto>> GetAllRezervariAsync()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hotels = await _hotelRepository.GetAllHotels();
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var tipCamere = await _hotelRepository.GetAllTipCamereAsync();

            var rezervariDto = new List<GetAllRezervariDto>();

            foreach (var rezervare in rezervari)
            {
                var camera = pretCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                if (camera == null)
                {
                    throw new Exception($"Nu a fost găsită o cameră corespunzătoare pentru RezervareId: {rezervare.RezervareId}");
                }

                var tipCamera = tipCamere.FirstOrDefault(tc => tc.TipCameraId == camera.TipCameraId);
                if (tipCamera == null)
                {
                    throw new Exception($"Nu a fost găsit un tip de cameră corespunzător pentru PretCameraId: {camera.PretCameraId}");
                }

                var hotel = hotels.FirstOrDefault(h => h.HotelId == tipCamera.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Nu a fost găsit un hotel corespunzător pentru TipCameraId: {tipCamera.TipCameraId}");
                }

                var rezervareDto = new GetAllRezervariDto
                {
                    UserId = rezervare.UserId,
                    HotelName = hotel.Name,
                    CheckIn = rezervare.CheckIn,
                    CheckOut = rezervare.CheckOut,
                    Pret = (decimal)camera.PretNoapte,
                    Stare = rezervare.Stare.ToString(),
                };

                rezervariDto.Add(rezervareDto);
            }

            return rezervariDto;
        }

        // Obține rezervările care nu sunt expirate
        public async Task<List<GetAllRezervariDto>> GetNonExpiredRezervariAsync()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hoteluri = await _hotelRepository.GetAllHotels();
            var preturiCamere = await _hotelRepository.GetAllPretCamere();
            var tipuriCamere = await _hotelRepository.GetAllTipCamereAsync();

            var rezervariDto = new List<GetAllRezervariDto>();

            foreach (var rezervare in rezervari.Where(r => r.Stare != StareRezervare.Expirata))
            {
                var camera = preturiCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                if (camera == null)
                {
                    throw new Exception($"Nu a fost găsită o cameră asociată pentru RezervareId: {rezervare.RezervareId}");
                }

                var tipCamera = tipuriCamere.FirstOrDefault(tc => tc.TipCameraId == camera.TipCameraId);
                if (tipCamera == null)
                {
                    throw new Exception($"Nu a fost găsit un TipCamera asociat pentru PretCameraId: {camera.PretCameraId}");
                }

                var hotel = hoteluri.FirstOrDefault(h => h.HotelId == tipCamera.HotelId);
                if (hotel == null)
                {
                    throw new Exception($"Nu a fost găsit un Hotel asociat pentru TipCameraId: {tipCamera.TipCameraId}");
                }

                var rezervareDto = new GetAllRezervariDto
                {
                    UserId = rezervare.UserId,
                    HotelName = hotel.Name,
                    CheckIn = rezervare.CheckIn,
                    CheckOut = rezervare.CheckOut,
                    Pret = (decimal)camera.PretNoapte,
                    Stare = rezervare.Stare.ToString(),
                };

                rezervariDto.Add(rezervareDto);
            }

            return rezervariDto;
        }
    }
}
