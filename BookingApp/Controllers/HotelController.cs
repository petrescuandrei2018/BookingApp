using AutoMapper;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Models;
using Stripe;
using Microsoft.Extensions.Options;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IServiciuPlata _serviciuPlata;
        private readonly IHotelService _serviciuHotel;
        private readonly IAuthService _serviciuAutentificare;
        private readonly string _webhookSecret;

        public HotelController(
            IServiciuPlata serviciuPlata,
            IHotelService serviciuHotel,
            IAuthService serviciuAutentificare,
            IOptions<StripeSettings> stripeSettings)
        {
            _serviciuPlata = serviciuPlata;
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
            _webhookSecret = stripeSettings.Value.WebhookSecret;
        }

        [HttpPost("register")]
        public async Task<ResponseDto> Inregistreaza([FromBody] UserDto userDto)
        {
            var response = new ResponseDto();

            if (!ModelState.IsValid)
            {
                response.IsSuccess = false;
                response.Message = "Datele trimise nu sunt valide.";
                return response;
            }

            try
            {
                // Validare: Email unic
                var existaEmail = await _serviciuAutentificare.ExistaEmailAsync(userDto.Email);
                if (existaEmail)
                {
                    response.IsSuccess = false;
                    response.Message = "Email-ul este deja utilizat.";
                    return response;
                }

                // Validare: Telefon unic
                var existaTelefon = await _serviciuAutentificare.ExistaTelefonAsync(userDto.PhoneNumber);
                if (existaTelefon)
                {
                    response.IsSuccess = false;
                    response.Message = "Numărul de telefon este deja utilizat.";
                    return response;
                }

                // Validare: Dacă `Rol` nu este valid, setează implicit "user"
                if (!new[] { "admin", "user" }.Contains(userDto.Rol?.ToLower()))
                {
                    userDto.Rol = "user";
                }

                // Apelează metoda de înregistrare
                var utilizatorNou = await _serviciuAutentificare.RegisterUser(userDto);
                response.IsSuccess = true;
                response.Message = "Utilizator înregistrat cu succes.";
                response.Result = new
                {
                    Id = utilizatorNou.UserId,
                    Nume = utilizatorNou.UserName,
                    Email = utilizatorNou.Email,
                    Telefon = utilizatorNou.PhoneNumber,
                    Varsta = utilizatorNou.Varsta,
                    Rol = utilizatorNou.Rol
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }





        [HttpPost("login")]
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

        [HttpPost("CreateRezervare")]
        [Authorize]
        public async Task<IActionResult> Rezerva([FromBody] RezervareDto rezervareDto)
        {
            var response = new ResponseDto();

            try
            {
                // Obține ID-ul utilizatorului logat din token
                var utilizatorId = int.Parse(User.Claims.First(c => c.Type == "UtilizatorId").Value);

                // Creează rezervarea
                response.Result = await _serviciuHotel.CreateRezervareFromDto(rezervareDto, utilizatorId);
                response.IsSuccess = true;
                response.Message = "Rezervarea a fost creată cu succes.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }




        [HttpGet("GetAllRezervari")]
        [Authorize]
        public async Task<ResponseDto> GetAllRezervari()
        {
            var response = new ResponseDto();
            try
            {
                response.Result = await _serviciuHotel.GetAllRezervariAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet("GetAllHotels")]
        public async Task<ResponseDto> GetAllHotels()
        {
            var response = new ResponseDto();
            try
            {
                response.Result = await _serviciuHotel.GetAllHotels();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet("GetAllHotels/{filtruNume?}")]
        public async Task<IActionResult> GetAllHotels(string? filtruNume)
        {
            var hoteluri = await _serviciuHotel.GetAllHotels(filtruNume);
            return Ok(hoteluri);
        }

        [HttpPut("SetAdmin")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetAdmin(int userId, bool isAdmin)
        {
            if (userId <= 0)
            {
                return BadRequest(new { Mesaj = "ID-ul utilizatorului nu este valid." });
            }

            try
            {
                var user = await _serviciuAutentificare.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Mesaj = "Utilizatorul nu a fost găsit." });
                }

                user.Rol = isAdmin ? "admin" : "user";
                var success = await _serviciuAutentificare.UpdateUserAsync(user);

                if (!success)
                {
                    return StatusCode(500, new { Mesaj = "A apărut o eroare la salvarea modificărilor." });
                }

                return Ok(new { Mesaj = $"Rolul utilizatorului {user.UserName} a fost actualizat cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _serviciuAutentificare.GetAllUsersAsync();
                return Ok(users.Select(user => new
                {
                    Id = user.UserId,
                    Name = user.UserName,
                    Email = user.Email,
                    Rol = user.Rol.ToString()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
        }

        [HttpGet("GetAllHotelsTipCamere")]
        public async Task<IActionResult> GetAllHotelsTipCamere()
        {
            var tipuriCamere = await _serviciuHotel.GetAllHotelsTipCamera();
            return Ok(tipuriCamere);
        }

        [HttpGet("GetNonExpiredRezervari")]
        public async Task<IActionResult> GetNonExpiredRezervari()
        {
            var rezervari = await _serviciuHotel.GetNonExpiredRezervari();
            return Ok(rezervari);
        }

        [HttpPost("Plata")]
        public async Task<IActionResult> ProcesarePlata([FromQuery] int rezervareId, [FromQuery] decimal suma)
        {
            if (rezervareId <= 0 || suma <= 0)
            {
                return BadRequest(new { Mesaj = "ID-ul rezervării și suma trebuie să fie valide." });
            }

            try
            {
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return BadRequest(new { Mesaj = $"Rezervarea cu ID-ul {rezervareId} nu există." });
                }

                rezervare.CalculareSumaTotala();

                if (suma > rezervare.SumaRamasaDePlata)
                {
                    return BadRequest(new { Mesaj = "Suma depășește suma totală rămasă de plată." });
                }

                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "RON", "Plată rezervare");
                rezervare.SumaRamasaDePlata -= suma;
                rezervare.ClientSecret = clientSecret;

                if (rezervare.SumaRamasaDePlata <= 0)
                {
                    rezervare.StarePlata = "Platita";
                }

                await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);

                return Ok(new
                {
                    Mesaj = rezervare.SumaRamasaDePlata > 0 ? "Plata procesată parțial." : "Plata procesată integral.",
                    SumaRamasaDePlata = rezervare.SumaRamasaDePlata,
                    ClientSecret = clientSecret
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea plății: {ex.Message}" });
            }
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> ProcesareRefund([FromQuery] string paymentIntentId, [FromQuery] decimal? suma)
        {
            if (string.IsNullOrEmpty(paymentIntentId))
            {
                return BadRequest(new { Mesaj = "ID-ul intenției de plată este necesar." });
            }

            try
            {
                var statusRefund = await _serviciuPlata.ProceseazaRefundAsync(paymentIntentId, suma);
                return Ok(new { Mesaj = "Refund procesat cu succes.", Status = statusRefund });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea refund-ului: {ex.Message}" });
            }
        }
    }
}

