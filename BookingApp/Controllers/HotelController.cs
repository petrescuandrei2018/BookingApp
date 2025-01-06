using AutoMapper;
using BookingApp.Data;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Models;
using Swashbuckle.AspNetCore.Annotations;
using BookingApp.Helpers;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        // Răspuns general pentru toate metodele
        private ResponseDto _responseSMR;

        // Serviciul pentru gestionarea hotelurilor
        private IHotelService _serviciuHotel;

        // Serviciul pentru autentificare
        private readonly IAuthService _serviciuAutentificare;

        // Constructor pentru inițializarea dependențelor
        public HotelController(AppDbContext database, IMapper mapper, IHotelService serviciuHotel, IAuthService serviciuAutentificare)
        {
            _responseSMR = new ResponseDto();
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
        }

        // Endpoint pentru înregistrarea unui utilizator nou
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
                // Crearea unui utilizator nou
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

        // Endpoint pentru obținerea tuturor hotelurilor
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

        // Endpoint pentru obținerea hotelurilor după filtru de nume
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

        // Endpoint protejat pentru obținerea tuturor tipurilor de camere din hoteluri
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

        // Endpoint pentru filtrarea hotelurilor după tipul camerei și capacitate
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

        // Endpoint pentru obținerea tuturor camerelor cu prețuri din hoteluri
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

        // Endpoint pentru obținerea camerelor după filtrul de preț, nume și capacitate
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

        // Endpoint pentru obținerea hotelurilor pe baza ratingului
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

        // Endpoint pentru crearea unei rezervări noi
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

        // Endpoint pentru obținerea tuturor rezervărilor
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

        // Endpoint protejat pentru obținerea rezervărilor neexpirate
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

        // Endpoint pentru autentificarea utilizatorilor
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

        /// <summary>
        /// Modifică rolul unui utilizator (Admin/User)
        /// </summary>
        /// <param name="userId">ID-ul utilizatorului selectat</param>
        /// <param name="isAdmin">True pentru Admin, False pentru User</param>
        [HttpPut]
        [Route("SetAdmin")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "SetAdmin")]
        public async Task<IActionResult> SetAdmin(int userId, bool isAdmin)
        {
            try
            {
                // Verificăm dacă utilizatorul există în baza de date
                var user = await _serviciuAutentificare.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "Utilizatorul nu a fost găsit." });
                }

                // Actualizăm rolul utilizatorului
                user.Rol = isAdmin ? RolUtilizator.Admin : RolUtilizator.User;

                // Salvăm modificările în baza de date
                var success = await _serviciuAutentificare.UpdateUserAsync(user);
                if (!success)
                {
                    return StatusCode(500, new { Message = "A apărut o eroare la salvarea modificărilor." });
                }

                return Ok(new { Message = $"Rolul utilizatorului {user.UserName} a fost actualizat cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"A apărut o eroare: {ex.Message}" });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Preluăm lista de utilizatori din serviciul de autentificare
                var users = await _serviciuAutentificare.GetAllUsersAsync();

                // Mapăm utilizatorii în DTO-uri pentru a returna doar informațiile relevante
                var result = users.Select(user => new
                {
                    Id = user.UserId,
                    Name = user.UserName,
                    Email = user.Email,
                    Rol = user.Rol.ToString() // Convertim enum-ul în string
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"A apărut o eroare: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("GetUsersForDropdown")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersForDropdown()
        {
            try
            {
                // Preluăm utilizatorii din baza de date
                var users = await _serviciuAutentificare.GetAllUsersAsync();

                // Transformăm utilizatorii într-un format simplu pentru dropdown
                var userDropdownList = users.Select(u => new
                {
                    Value = u.UserId, // ID-ul utilizatorului
                    Label = $"{u.UserId} - {u.UserName} ({u.Rol})" // Textul afișat
                }).ToList();

                return Ok(userDropdownList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"A apărut o eroare: {ex.Message}" });
            }
        }

        [HttpGet("GetDropdownUsers")]
        [ApiExplorerSettings(IgnoreApi = true)] // Ascunde acest endpoint din Swagger
        public IActionResult GetDropdownUsers()
        {
            // Simulează obținerea listei de utilizatori
            var utilizatori = UserDropdownCache.Users.Select(u => new
            {
                Id = u.UserId,
                DisplayName = $"{u.DisplayName}"
            }).ToList();

            return Ok(utilizatori);
        }

    }
}
