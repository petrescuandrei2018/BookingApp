﻿using AutoMapper;
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
                // Verifică validitatea sumei
                if (suma <= 0)
                {
                    return BadRequest(new { Mesaj = "Suma trebuie să fie mai mare decât zero." });
                }

                // Preia rezervarea din BD
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return BadRequest(new { Mesaj = $"Rezervarea cu ID-ul {rezervareId} nu există." });
                }

                // Recalculează suma totală dacă este necesar
                rezervare.CalculareSumaTotala();

                // Verifică dacă suma este mai mare decât suma rămasă de plată
                if (suma > rezervare.SumaRamasaDePlata)
                {
                    return BadRequest(new { Mesaj = "Suma introdusă depășește suma totală rămasă de plată." });
                }

                // Inițiază plata cu Stripe
                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "RON", "Plată rezervare");
                rezervare.SumaRamasaDePlata -= suma;
                rezervare.ClientSecret = clientSecret;

                // Actualizează rezervarea în BD
                await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);

                return Ok(new
                {
                    Mesaj = "Plata inițiată cu succes.",
                    SumaRamasaDePlata = rezervare.SumaRamasaDePlata,
                    ClientSecret = clientSecret
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
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


        [HttpPost("ProcesarePlataStripe")]
        public async Task<IActionResult> ProcesarePlataStripe(int rezervareId)
        {
            try
            {
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return BadRequest(new { Mesaj = $"Rezervarea cu ID-ul {rezervareId} nu există." });
                }

                // Verifică dacă există restanțe
                if (rezervare.SumaRamasaDePlata > 0)
                {
                    return Ok(new { Mesaj = "Plata procesată parțial. Mai sunt de achitat: " + rezervare.SumaRamasaDePlata + " RON." });
                }

                rezervare.StarePlata = "Platita";
                await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);

                return Ok(new { Mesaj = "Plata procesată integral cu succes." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = $"Eroare la procesarea plății: {ex.Message}" });
            }
        }


        [HttpPost("FinalizeazaPlata")]
        public async Task<IActionResult> FinalizeazaPlata(int rezervareId)
        {
            try
            {
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return BadRequest(new { Mesaj = $"Rezervarea cu ID-ul {rezervareId} nu există." });
                }

                if (rezervare.SumaRamasaDePlata > 0)
                {
                    return BadRequest(new { Mesaj = "Plata nu este completă. Mai sunt de achitat: " + rezervare.SumaRamasaDePlata + " RON." });
                }

                rezervare.StarePlata = "Platita";
                await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);

                return Ok(new { Mesaj = "Plata finalizată cu succes." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = $"Eroare la finalizarea plății: {ex.Message}" });
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
