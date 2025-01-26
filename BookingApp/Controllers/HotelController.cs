using AutoMapper;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Models;
using Stripe;
using Microsoft.Extensions.Options;
using BookingApp.Services;

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
        private readonly IServiciuCacheRedis _cacheRedis;
        private readonly IServiciuEmail _emailMock;
        private readonly IServiciuEmail _emailReal;

        public HotelController(
            IServiciuPlata serviciuPlata,
            IHotelService serviciuHotel,
            IAuthService serviciuAutentificare,
            IOptions<StripeSettings> stripeSettings,
            IServiciuCacheRedis cacheRedis,
            [FromServices] ServiciuEmailMock emailMock,
            [FromServices] ServiciuEmailSmtp emailReal)
        {
            _serviciuPlata = serviciuPlata;
            _serviciuHotel = serviciuHotel;
            _serviciuAutentificare = serviciuAutentificare;
            _webhookSecret = stripeSettings.Value.WebhookSecret;
            _cacheRedis = cacheRedis;
            _emailMock = emailMock;
            _emailReal = emailReal;
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
        public async Task<IActionResult> CreateRezervare([FromBody] RezervareDto rezervareDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Mesaj = "Datele trimise nu sunt valide." });
            }

            try
            {
                // Obținem ID-ul utilizatorului conectat
                rezervareDto.UserId = int.Parse(User.Claims.First(c => c.Type == "UtilizatorId").Value);

                // Apelăm metoda din HotelService pentru a crea rezervarea
                var rezultat = await _serviciuHotel.CreateRezervareFromDto(rezervareDto);

                return Ok(new
                {
                    Mesaj = "Rezervarea a fost creată cu succes.",
                    Rezervare = rezultat
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
        }


        [HttpGet("GetAllRezervari")]
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

        [HttpGet("GetRezervariEligibilePlata")]
        public async Task<IActionResult> GetRezervariEligibilePlata()
        {
            try
            {
                // Obținem toate rezervările din serviciul hotel
                var rezervariEligibile = await _serviciuHotel.GetAllRezervariAsync();

                // Procesăm fiecare rezervare pentru a calcula SumaRamasaDePlata
                var rezultat = new List<object>();

                foreach (var r in rezervariEligibile)
                {
                    var sumaAchitata = await _serviciuHotel.ObțineSumaAchitatăAsync(r.RezervareId);

                    var sumaTotalaDePlata = r.Pret.HasValue
                        ? r.Pret.Value * (r.CheckOut - r.CheckIn).Days
                        : 0;

                    var sumaRamasaDePlata = sumaTotalaDePlata - sumaAchitata;

                    if (r.Stare == "Viitoare" && sumaRamasaDePlata > 0)
                    {
                        rezultat.Add(new
                        {
                            r.RezervareId,
                            r.UserId,
                            r.HotelName,
                            r.NumeCamera,
                            r.CheckIn,
                            r.CheckOut,
                            r.Pret,
                            r.Stare,
                            SumaTotalaDePlata = sumaTotalaDePlata,
                            SumaAchitata = sumaAchitata,
                            SumaRamasaDePlata = sumaRamasaDePlata
                        });
                    }
                }

                if (!rezultat.Any())
                {
                    return NotFound(new { Mesaj = "Nu există rezervări eligibile pentru plată." });
                }

                return Ok(rezultat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
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
                // Obține rezervarea din DB
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return NotFound(new { Mesaj = "Rezervarea nu există." });
                }

                // Validare: Permitem plata doar dacă CheckIn este în viitor
                if (rezervare.CheckIn <= DateTime.Now)
                {
                    rezervare.Stare = "Expirata";

                    if (rezervare.SumaAchitata > 0)
                    {
                        // Refund automat pentru suma deja plătită
                        await _serviciuPlata.ProceseazaRefundAsync(rezervare.ClientSecret, rezervare.SumaAchitata);
                        rezervare.SumaAchitata = 0;
                        rezervare.SumaRamasaDePlata = rezervare.SumaTotala;
                        rezervare.StarePlata = "Refundata";
                    }

                    await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);
                    return BadRequest(new { Mesaj = "Rezervarea este expirată. Nu se mai pot face plăți." });
                }

                // Continuăm cu procesarea plății dacă este validă
                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "RON", "Plată rezervare");
                rezervare.SumaAchitata += suma;
                rezervare.SumaRamasaDePlata -= suma;
                rezervare.ClientSecret = clientSecret;

                // Actualizăm starea plății
                rezervare.StarePlata = rezervare.SumaRamasaDePlata == 0 ? "Platita" : "In Progress";
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


        [HttpGet("GetRezervariEligibileRefund")]
        public async Task<IActionResult> GetRezervariEligibileRefund()
        {
            try
            {
                // Apelează serviciul pentru a obține rezervările eligibile
                var rezervariEligibile = await _serviciuHotel.GetRezervariEligibileRefund();

                // Returnează rezultatul
                return Ok(rezervariEligibile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la obținerea rezervărilor eligibile pentru refund: {ex.Message}" });
            }
        }

        // Rută pentru trimiterea emailului folosind mock
        [HttpPost("TrimiteEmailMock")]
        public async Task<IActionResult> TrimiteEmailMock([FromQuery] string destinatar, [FromQuery] string subiect, [FromQuery] string mesaj)
        {
            try
            {
                await _emailMock.TrimiteEmailAsync(destinatar, subiect, mesaj);
                return Ok(new { Mesaj = "Email (mock) trimis cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la trimiterea emailului (mock): {ex.Message}" });
            }
        }

        // Rută pentru trimiterea emailului folosind real
        [HttpPost("TrimiteEmailReal")]
        public async Task<IActionResult> TrimiteEmailReal([FromQuery] string destinatar, [FromQuery] string subiect, [FromQuery] string mesaj)
        {
            try
            {
                await _emailReal.TrimiteEmailAsync(destinatar, subiect, mesaj);
                return Ok(new { Mesaj = "Email (real) trimis cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la trimiterea emailului (real): {ex.Message}" });
            }
        }
        [HttpPost("NotificarePlataIntegrala")]
        public async Task<IActionResult> NotificarePlataIntegrala([FromQuery] int rezervareId)
        {
            if (rezervareId <= 0)
            {
                return BadRequest(new { Mesaj = "ID-ul rezervării nu este valid." });
            }

            try
            {
                // Obține rezervarea din baza de date
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return NotFound(new { Mesaj = "Rezervarea nu există." });
                }

                // Verifică dacă plata este completă
                if (rezervare.SumaRamasaDePlata > 0)
                {
                    return BadRequest(new
                    {
                        Mesaj = "Plata nu este completă.",
                        SumaRamasaDePlata = rezervare.SumaRamasaDePlata
                    });
                }

                if (rezervare.StarePlata != "Platita")
                {
                    rezervare.StarePlata = "Platita"; // Actualizează starea plății dacă e cazul
                    await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);
                }

                // Setează o intrare în Redis pentru notificare
                var cheieRedis = $"notificare:rezervare:{rezervare.RezervareId}";
                await _cacheRedis.SeteazaValoareAsync(cheieRedis, new
                {
                    rezervare.UserId,
                    rezervare.RezervareId,
                    rezervare.StarePlata
                }, TimeSpan.FromMinutes(10));

                // Trimitere notificare reală (folosind SMTP)
                await _emailReal.TrimiteEmailAsync(
                    "emailul.utilizatorului@exemplu.com",
                    "Plata completă confirmată",
                    $"Rezervarea cu ID-ul {rezervare.RezervareId} a fost plătită integral."
                );

                // Șterge intrarea din Redis după notificare
                await _cacheRedis.StergeValoareAsync(cheieRedis);

                return Ok(new { Mesaj = "Notificare trimisă cu succes pentru plata integrală." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea notificării: {ex.Message}" });
            }
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> ProcesareRefundByRezervareId([FromQuery] int rezervareId, [FromQuery] decimal sumaRefund)
        {
            if (rezervareId <= 0)
            {
                return BadRequest(new { Mesaj = "ID-ul rezervării este necesar." });
            }

            try
            {
                // 1. Obține rezervarea din baza de date
                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return NotFound(new { Mesaj = "Rezervarea nu există." });
                }

                // 2. Verifică dacă suma refund-ului este validă
                if (sumaRefund <= 0 || sumaRefund > rezervare.SumaAchitata)
                {
                    return BadRequest(new { Mesaj = "Suma refund-ului este invalidă. Verificați suma achitată." });
                }

                // 3. Procesează refund-ul în Stripe
                var statusRefund = await _serviciuPlata.ProceseazaRefundAsync(rezervare.ClientSecret, sumaRefund);

                // 4. Actualizează datele rezervării în baza de date
                rezervare.SumaAchitata -= sumaRefund;
                rezervare.SumaRamasaDePlata += sumaRefund;

                // Actualizează starea plății
                if (rezervare.SumaAchitata == 0)
                {
                    rezervare.StarePlata = "Refundata";
                }
                else if (rezervare.SumaAchitata > 0 && rezervare.SumaRamasaDePlata > 0)
                {
                    rezervare.StarePlata = "In Progress";
                }

                await _serviciuHotel.ActualizeazaRezervareAsync(rezervare);

                // 5. Returnează rezultatul refund-ului
                return Ok(new
                {
                    Mesaj = "Refund procesat cu succes.",
                    Status = statusRefund,
                    SumaAchitataNoua = rezervare.SumaAchitata,
                    SumaRamasaDePlataNoua = rezervare.SumaRamasaDePlata,
                    StarePlataNoua = rezervare.StarePlata
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la procesarea refund-ului: {ex.Message}" });
            }
        }

    }
}

