using AutoMapper;
using BookingApp.Data;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Services;


namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private ResponseDto _responseSMR;
        private IHotelService _serviciuHotel;
        private readonly IAuthService _serviciuAutentificare;  // Injectăm serviciul de autentificare

        public HotelController(AppDbContext database, IMapper mapper, IHotelService serviciuHotel, IAuthService serviciuAutentificare)
        {
            _responseSMR = new ResponseDto();
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;  // Inițializăm serviciul de autentificare
        }

        /// Endpoint pentru înregistrarea unui utilizator
        [HttpPost]
        [Route("register")]
        public async Task<ResponseDto> Inregistreaza([FromBody] UserDto userDto)
        {
            if (ModelState.IsValid == false)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = "Datele trimise nu sunt valide.";
                _responseSMR.Result = null;
                return _responseSMR;
            }

            try
            {
                // Înregistrăm utilizatorul folosind serviciul de autentificare
                var utilizatorNou = await _serviciuAutentificare.RegisterUser(userDto);

                // Debugging: Check the contents of utilizatorNou
                if (utilizatorNou == null)
                {
                    throw new Exception("Eroare la înregistrare. Utilizatorul returnat este null.");
                }

                Console.WriteLine($"Utilizator înregistrat: {utilizatorNou.UserId}, {utilizatorNou.UserName}");

                // Construim răspunsul cu detaliile utilizatorului
                _responseSMR.IsSuccess = true;
                _responseSMR.Message = "Utilizator înregistrat cu succes.";
                _responseSMR.Result = new
                {
                    Id = utilizatorNou.UserId,
                    Nume = utilizatorNou.UserName,
                    Email = utilizatorNou.Email,
                    Telefon = utilizatorNou.PhoneNumber,
                    Varsta = utilizatorNou.Varsta
                };
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetAllHotels")]
        public async Task<ResponseDto> GetAllHotels()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotels();
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetAllHotels/{filtruNume?}")]
        public async Task<ResponseDto> GetAllHotels(string? filtruNume)
        {
            try
            {
               _responseSMR.Result = await _serviciuHotel.GetAllHotels(filtruNume);
                return _responseSMR ;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetAllHotelsTipCamere")]
        public async Task<ResponseDto> GetAllHotelsTipCamere()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCamera();
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetHotelsTipCamere/{filtruNumeHotel?}/{capacitatePersoane?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamere(string? filtruNumeHotel="", int? capacitatePersoane=0)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraFiltered(filtruNumeHotel, capacitatePersoane);
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetAllHotelsTipCamerePret")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraPret();
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetHotelsTipCamerePret/{filtruNumeHotel?}/{capacitatePersoane?}/{pretCamera?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret(string? filtruNumeHotel = "", int? capacitatePersoane = 0, 
            float? pretCamera=0)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraPretFiltered(filtruNumeHotel, capacitatePersoane, pretCamera);
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [HttpGet]
        [Route("GetAllHotelsWithRatings/{rating?}")]
        public async Task<ResponseDto> GetAllHotelsWithRatings(double? rating)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsByRating(rating);
                return _responseSMR;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }
        [HttpPost]
        [Route("CreateRezervare")]
        [Authorize] // Permite accesul doar utilizatorilor autentificați
        public async Task<ResponseDto> Rezerva([FromBody] RezervareDto rezervareDto)
        {
            var response = new ResponseDto(); // Inițializăm răspunsul

            try
            {
                // Verificăm dacă utilizatorul este autentificat și obținem UserId din token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UtilizatorId");
                if (userIdClaim == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Token invalid sau utilizator neautorizat.";
                    response.Result = null;
                    return response;
                }

                // Convertim UserId din token la int
                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    response.IsSuccess = false;
                    response.Message = "Token-ul conține un ID utilizator invalid.";
                    response.Result = null;
                    return response;
                }

                // Validăm că UserId din token corespunde cu cel din rezervareDto
                if (userId != rezervareDto.UserId)
                {
                    response.IsSuccess = false;
                    response.Message = "Utilizatorul autentificat nu are permisiuni pentru această acțiune.";
                    response.Result = null;
                    return response;
                }

                // Apelăm serviciul pentru a crea rezervarea
                response.Result = await _serviciuHotel.CreateRezervareFromDto(rezervareDto);
                response.IsSuccess = true;
                response.Message = "Rezervarea a fost creată cu succes.";
            }
            catch (Exception ex)
            {
                // Gestionăm erorile și returnăm mesajul corespunzător
                response.IsSuccess = false;
                response.Message = $"Eroare: {ex.Message}";
                response.Result = null;
            }

            return response;
        }

        [HttpGet]
        [Route("GetAllRezervari")]
        public async Task<ResponseDto> GetAllRezervari()
        {
            try
            {
                // Apelarea serviciului pentru a obține toate rezervările
                _responseSMR.Result = await _serviciuHotel.GetAllRezervariAsync();
                return _responseSMR;
            }
            catch (Exception ex)
            {
                // Gestionarea erorilor
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetNonExpiredRezervari")]
        public async Task<ResponseDto> GetNonExpiredRezervari()
        {
            try
            {
                // Apelăm metoda din serviciu pentru a obține rezervările care nu sunt expirate
                _responseSMR.Result = await _serviciuHotel.GetNonExpiredRezervariAsync();
                _responseSMR.IsSuccess = true;
                return _responseSMR;
            }
            catch (Exception ex)
            {
                // Gestionăm erorile și returnăm mesajul corespunzător
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
                _responseSMR.Result = null;
                return _responseSMR;
            }
        }

        /// Autentifică utilizatorul și generează un token JWT.
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LogInDto logInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datele trimise nu sunt valide.");
            }

            var token = _serviciuAutentificare.AutentificaUtilizator(logInDto.Email, logInDto.Password);

            if (token == "Email sau parola incorectă.")
            {
                return Unauthorized(new
                {
                    isSuccess = false,
                    message = "Email sau parola incorectă."
                });
            }

            return Ok(new
            {
                isSuccess = true,
                message = "Autentificare reușită.",
                result = token
            });
        }
    }
}
