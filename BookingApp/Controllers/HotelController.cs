using AutoMapper;
using BookingApp.Data;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private ResponseDto _responseSMR;
        private IHotelService _serviciuHotel;
        private readonly IAuthService _serviciuAutentificare;

        public HotelController(AppDbContext database, IMapper mapper, IHotelService serviciuHotel, IAuthService serviciuAutentificare)
        {
            _responseSMR = new ResponseDto();
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ResponseDto> Inregistreaza([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = "Datele trimise nu sunt valide.";
                return _responseSMR;
            }

            try
            {
                var utilizatorNou = await _serviciuAutentificare.RegisterUser(userDto);

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
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetAllHotels")]
        public async Task<ResponseDto> GetAllHotels()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotels();
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetAllHotels/{filtruNume?}")]
        public async Task<ResponseDto> GetAllHotels(string? filtruNume)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotels(filtruNume);
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllHotelsTipCamere")]
        public async Task<ResponseDto> GetAllHotelsTipCamere()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCamera();
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetHotelsTipCamere/{filtruNumeHotel?}/{capacitatePersoane?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamere(string? filtruNumeHotel = "", int? capacitatePersoane = 0)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraFiltered(filtruNumeHotel, capacitatePersoane);
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetAllHotelsTipCamerePret")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraPret();
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetHotelsTipCamerePret/{filtruNumeHotel?}/{capacitatePersoane?}/{pretCamera?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret(string? filtruNumeHotel = "", int? capacitatePersoane = 0, float? pretCamera = 0)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsTipCameraPretFiltered(filtruNumeHotel, capacitatePersoane, pretCamera);
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpGet]
        [Route("GetAllHotelsWithRatings/{rating?}")]
        public async Task<ResponseDto> GetAllHotelsWithRatings(double? rating)
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllHotelsByRating(rating);
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [HttpPost]
        [Route("CreateRezervare")]
        [Authorize]
        public async Task<ResponseDto> Rezerva([FromBody] RezervareDto rezervareDto)
        {
            var response = new ResponseDto();

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UtilizatorId");
                if (userIdClaim == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Token invalid sau utilizator neautorizat.";
                    return response;
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    response.IsSuccess = false;
                    response.Message = "Token-ul conține un ID utilizator invalid.";
                    return response;
                }

                if (userId != rezervareDto.UserId)
                {
                    response.IsSuccess = false;
                    response.Message = "Utilizatorul autentificat nu are permisiuni pentru această acțiune.";
                    return response;
                }

                response.Result = await _serviciuHotel.CreateRezervareFromDto(rezervareDto);
                response.IsSuccess = true;
                response.Message = "Rezervarea a fost creată cu succes.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("GetAllRezervari")]
        public async Task<ResponseDto> GetAllRezervari()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetAllRezervariAsync();
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

        [Authorize]
        [HttpGet]
        [Route("GetNonExpiredRezervari")]
        public async Task<ResponseDto> GetNonExpiredRezervari()
        {
            try
            {
                _responseSMR.Result = await _serviciuHotel.GetNonExpiredRezervariAsync();
                _responseSMR.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseSMR.IsSuccess = false;
                _responseSMR.Message = ex.Message;
            }

            return _responseSMR;
        }

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
