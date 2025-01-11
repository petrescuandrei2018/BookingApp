using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;
using BookingApp.Repository.Abstractions;
using BookingApp.Services.Abstractions;
using System.Linq;

namespace BookingApp.Services
{
    // Serviciu pentru gestionarea operațiunilor legate de hoteluri
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        // Constructor pentru injectarea dependențelor
        public HotelService(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        // Metodă pentru procesarea plății Stripe pentru o rezervare
        public async Task ProcesarePlataStripeAsync(int rezervareId)
        {
            // Obținem rezervarea pe baza ID-ului
            var rezervare = await _hotelRepository.GetRezervareByIdAsync(rezervareId);
            if (rezervare == null)
            {
                throw new Exception($"Rezervarea cu ID-ul {rezervareId} nu a fost găsită.");
            }

            // Actualizăm starea plății
            rezervare.StarePlata = "Platita";
            await _hotelRepository.ActualizeazaRezervareAsync(rezervare);
        }

        // Metodă pentru crearea unei rezervări pornind de la DTO
        public async Task<RezervareDto> CreateRezervareFromDto(RezervareDto rezervareDto)
        {
            // Validare: CheckOut trebuie să fie după CheckIn
            if (rezervareDto.CheckOut < rezervareDto.CheckIn)
            {
                throw new Exception("Data de CheckOut nu poate fi mai mică decât data de CheckIn.");
            }

            // Verificăm existența utilizatorului
            var users = await _hotelRepository.GetAllUsers();
            if (!users.Any(x => x.UserId == rezervareDto.UserId))
            {
                throw new Exception($"Utilizatorul cu ID {rezervareDto.UserId} nu există.");
            }

            // Verificăm existența camerei și disponibilitatea acesteia
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var validPretCamera = pretCamere.FirstOrDefault(x => x.PretCameraId == rezervareDto.PretCameraId);
            if (validPretCamera == null)
            {
                throw new Exception($"Camera cu ID {rezervareDto.PretCameraId} nu există.");
            }

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

            // Mapăm DTO-ul în entitate
            var rezervare = _mapper.Map<Rezervare>(rezervareDto);

            // Stabilim starea rezervării
            rezervare.Stare = rezervareDto.CheckOut < DateTime.UtcNow
                ? StareRezervare.Expirata
                : rezervareDto.CheckIn <= DateTime.UtcNow
                    ? StareRezervare.Activa
                    : StareRezervare.Viitoare;

            // Salvăm rezervarea
            await _hotelRepository.AdaugaRezervare(rezervare, rezervareTipCameraId);

            return rezervareDto;
        }

        // Obține lista tuturor hotelurilor
        public async Task<List<ResponseHotelDto>> GetAllHotels()
        {
            var hotels = await _hotelRepository.GetAllHotels();
            return _mapper.Map<List<ResponseHotelDto>>(hotels);
        }

        // Obține lista hotelurilor filtrată după nume
        public async Task<List<ResponseHotelDto>> GetAllHotels(string? filtruNume)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            if (!string.IsNullOrEmpty(filtruNume))
            {
                hotels = hotels.Where(x => x.Name == filtruNume).ToList();
            }
            return _mapper.Map<List<ResponseHotelDto>>(hotels);
        }

        // Obține lista hotelurilor împreună cu rating-urile acestora
        public async Task<List<HotelWithRating>> GetAllHotelsByRating(double? rating)
        {
            var hotels = await _hotelRepository.GetAllHotels();
            var reviews = await _hotelRepository.GetAllReviews();

            return hotels.Select(hotel =>
            {
                var hotelReviews = reviews.Where(r => r.HotelId == hotel.HotelId);
                var ratingMediu = hotelReviews.Any() ? hotelReviews.Average(r => r.Rating) : 0;

                return new HotelWithRating
                {
                    Address = hotel.Address,
                    HotelName = hotel.Name,
                    ReviewuriTotale = hotelReviews.Count(),
                    Rating = ratingMediu
                };
            })
            .Where(hw => rating == null || hw.Rating >= rating)
            .ToList();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCamera()
        {
            return await _hotelRepository.GetAllHotelsTipCamera();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere, filtrată
        public async Task<List<HotelTipCamera>> GetAllHotelsTipCameraFiltered(string? filtruNumeHotel, int? capacitatePersoane)
        {
            var hotelsTipCamere = await _hotelRepository.GetAllHotelsTipCamera();
            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane.HasValue)
            {
                hotelsTipCamere = hotelsTipCamere
                    .Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane)
                    .ToList();
            }
            return hotelsTipCamere;
        }

        // Obține lista hotelurilor împreună cu tipurile de camere și prețurile acestora
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPret()
        {
            return await _hotelRepository.GetAllHotelsTipCameraPret();
        }

        // Obține lista hotelurilor împreună cu tipurile de camere și prețurile acestora, filtrată
        public async Task<List<HotelTipCameraPret>> GetAllHotelsTipCameraPretFiltered(string? filtruNumeHotel, int? capacitatePersoane, float? pretCamera)
        {
            var hotelsTipCamerePret = await _hotelRepository.GetAllHotelsTipCameraPret();
            if (!string.IsNullOrEmpty(filtruNumeHotel) && capacitatePersoane.HasValue && pretCamera.HasValue)
            {
                hotelsTipCamerePret = hotelsTipCamerePret
                    .Where(x => x.HotelName == filtruNumeHotel && x.CapacitatePersoane == capacitatePersoane && x.PretNoapte <= pretCamera)
                    .ToList();
            }
            return hotelsTipCamerePret;
        }

        // Obține lista tuturor rezervărilor
        public async Task<List<GetAllRezervariDto>> GetAllRezervariAsync()
        {
            var rezervari = await _hotelRepository.GetAllRezervariAsync();
            var hotels = await _hotelRepository.GetAllHotels();
            var pretCamere = await _hotelRepository.GetAllPretCamere();
            var tipCamere = await _hotelRepository.GetAllTipCamereAsync();

            return rezervari.Select(rezervare =>
            {
                var camera = pretCamere.FirstOrDefault(pc => pc.PretCameraId == rezervare.PretCameraId);
                var tipCamera = tipCamere.FirstOrDefault(tc => tc.TipCameraId == camera?.TipCameraId);
                var hotel = hotels.FirstOrDefault(h => h.HotelId == tipCamera?.HotelId);

                return new GetAllRezervariDto
                {
                    UserId = rezervare.UserId,
                    HotelName = hotel?.Name,
                    CheckIn = rezervare.CheckIn,
                    CheckOut = rezervare.CheckOut,
                    Pret = (decimal)(camera?.PretNoapte ?? 0),
                    Stare = rezervare.Stare.ToString(),
                };
            }).ToList();
        }

        // Obține lista rezervărilor care nu sunt expirate
        public async Task<List<GetAllRezervariDto>> GetNonExpiredRezervariAsync()
        {
            var rezervari = await GetAllRezervariAsync();
            return rezervari.Where(r => r.Stare != StareRezervare.Expirata.ToString()).ToList();
        }
    }
}
