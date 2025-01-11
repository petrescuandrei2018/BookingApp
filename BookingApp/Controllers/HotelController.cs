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

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IServiciuPlata _serviciuPlata;
        private ResponseDto _responseSMR;
        private readonly IHotelService _serviciuHotel;
        private readonly IAuthService _serviciuAutentificare;

        public HotelController(
            IServiciuPlata serviciuPlata,
            IHotelService serviciuHotel,
            IAuthService serviciuAutentificare)
        {
            _responseSMR = new ResponseDto();
            _serviciuPlata = serviciuPlata;
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
        }

        [HttpPost("register")]
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

        [HttpPost("InitiazaPlata")]
        public async Task<IActionResult> InitiazaPlata(int rezervareId, decimal suma)
        {
            try
            {
                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "usd", "Plata pentru rezervare");
                return Ok(new { Mesaj = "Plata inițiată cu succes.", ClientSecret = clientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }

        [HttpPost("ProcesarePlataStripe")]
        public async Task<IActionResult> ProcesarePlataStripe()
        {
            try
            {
                var jsonStripe = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var evenimentStripe = EventUtility.ParseEvent(jsonStripe);

                if (evenimentStripe.Type == "payment_intent.succeeded")
                {
                    var intentPlata = evenimentStripe.Data.Object as PaymentIntent;

                    var rezervareId = int.Parse(intentPlata.Metadata["RezervareId"]);
                    await _serviciuHotel.ProcesarePlataStripeAsync(rezervareId);

                    var utilizator = await _serviciuAutentificare.GetUserByIdAsync(rezervareId); // Obține email-ul
                    if (utilizator != null)
                    {
                        await _serviciuPlata.TrimiteEmailConfirmare(
                            utilizator.Email,
                            "Plată Confirmată"
                        );
                    }

                    return Ok(new { Mesaj = "Plata a fost procesată cu succes." });
                }

                return BadRequest(new { Mesaj = "Tip de eveniment nesuportat." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea Stripe: {ex.Message}" });
            }
        }



        [HttpPost("finalizeaza-plata")]
        public async Task<IActionResult> FinalizeazaPlata(int rezervareId, decimal suma, string email)
        {
            try
            {
                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "usd", "Plata pentru rezervare");
                await _serviciuPlata.TrimiteEmailConfirmare(email, "Plata a fost procesată cu succes.");
                return Ok(new { Mesaj = "Plata procesată și email trimis.", ClientSecret = clientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }

        [HttpGet("GetAllHotels")]
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

        [HttpGet("GetAllRezervari")]
        [Authorize]
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
