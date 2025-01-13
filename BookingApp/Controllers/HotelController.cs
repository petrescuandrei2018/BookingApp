using AutoMapper;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Models;
using Swashbuckle.AspNetCore.Annotations;
using BookingApp.Helpers;
using Stripe;
using System.IO;
using System.Linq;
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
        private readonly Dictionary<int, string> _cachePlati; // Cache-ul pentru plăți


        public HotelController(
            IServiciuPlata serviciuPlata,
            IHotelService serviciuHotel,
            IAuthService serviciuAutentificare,
            IOptions<StripeSettings> stripeSettings, // Injectează setările Stripe
            Dictionary<int, string> cachePlati) // Injectăm cache-ul 
        {
            _serviciuPlata = serviciuPlata;
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
            _webhookSecret = stripeSettings.Value.WebhookSecret; // Preia valoarea secretului
            _cachePlati = cachePlati;
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
                var utilizatorNou = await _serviciuAutentificare.RegisterUser(userDto);
                response.IsSuccess = true;
                response.Message = "Utilizator înregistrat cu succes.";
                response.Result = new
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
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
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

        // Endpoint pentru crearea unei rezervări noi
        [HttpPost]
        [Route("CreateRezervare")]
        /*[Authorize]*/
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
                    response.Message = $"Utilizatorul {userId} autentificat este diferit de utilizatorul {rezervareDto.UserId} pentru care se rezerva.";
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


        [HttpPost("InitiazaPlata")]
        public async Task<IActionResult> InitiazaPlata(int rezervareId, decimal suma)
        {
            try
            {
                Console.WriteLine($"[InitiazaPlata] Pornim procesarea plății pentru RezervareId: {rezervareId}, Suma: {suma}");

                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "usd", "Plata pentru rezervare");

                Console.WriteLine($"[InitiazaPlata] ClientSecret primit: {clientSecret}");

                // Stocăm detaliile în cache
                _cachePlati[rezervareId] = clientSecret;

                Console.WriteLine($"[InitiazaPlata] Cache actualizat pentru RezervareId: {rezervareId}, ClientSecret: {clientSecret}");

                return Ok(new { Mesaj = "Plata inițiată cu succes.", ClientSecret = clientSecret });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InitiazaPlata] Eroare: {ex.Message}");
                return BadRequest(new { Mesaj = ex.Message });
            }
        }



        [HttpPost("ProcesarePlataStripe")]
        public async Task<IActionResult> ProcesarePlataStripe()
        {
            try
            {
                // Obține primul `rezervareId` din cache (simulare)
                if (!_cachePlati.Any())
                {
                    return BadRequest(new { Mesaj = "Nu există plăți inițiate pentru procesare." });
                }

                var rezervareId = _cachePlati.Keys.First();
                var clientSecret = _cachePlati[rezervareId];

                // Simulează un eveniment Stripe
                var evenimentStripe = new
                {
                    id = "evt_test",
                    @object = "event",
                    type = "payment_intent.succeeded",
                    data = new
                    {
                        @object = new
                        {
                            id = clientSecret,
                            metadata = new Dictionary<string, string>
                    {
                        { "RezervareId", rezervareId.ToString() }
                    }
                        }
                    }
                };

                // Procesăm evenimentul simulat
                await _serviciuHotel.ProcesarePlataStripeAsync(rezervareId);
                return Ok(new { Mesaj = "Plata procesată cu succes pentru rezervare." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea Stripe: {ex.Message}" });
            }
        }

        [HttpPost("FinalizeazaPlata")]
        public async Task<IActionResult> FinalizeazaPlata(int rezervareId)
        {
            try
            {
                // Verificăm dacă rezervarea există
                if (!_cachePlati.ContainsKey(rezervareId))
                {
                    return BadRequest(new { Mesaj = "Rezervarea nu a fost inițiată pentru plată." });
                }

                // Marcăm rezervarea ca plătită
                _cachePlati.Remove(rezervareId);
                return Ok(new { Mesaj = "Plata finalizată cu succes pentru rezervare." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la finalizarea plății: {ex.Message}" });
            }
        }

        [HttpPost("ProceseazaRefund")]
        public async Task<IActionResult> ProceseazaRefund([FromQuery] string paymentIntentId, [FromQuery] decimal? suma)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentIntentId))
                {
                    return BadRequest(new { Mesaj = "ID-ul intenției de plată este necesar." });
                }

                var statusRefund = await _serviciuPlata.ProceseazaRefundAsync(paymentIntentId, suma);

                return Ok(new { Mesaj = "Refund procesat cu succes.", Status = statusRefund });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea refund-ului: {ex.Message}" });
            }
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

        [HttpPut("SetAdmin")]
        [Authorize(Roles = "Admin")]
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

                user.Rol = isAdmin ? RolUtilizator.Admin : RolUtilizator.User;
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
    }
}
