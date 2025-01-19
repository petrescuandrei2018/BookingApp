using AutoMapper;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Models;
using Stripe;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IServiciuPlata _serviciuPlata;
        private readonly IHotelService _serviciuHotel;

        public HotelController(IServiciuPlata serviciuPlata, IHotelService serviciuHotel)
        {
            _serviciuPlata = serviciuPlata;
            _serviciuHotel = serviciuHotel;
        }

        // Endpoint pentru inițierea unei plăți
        [HttpPost("InitiazaPlata")]
        public async Task<IActionResult> InitiazaPlata(int rezervareId, decimal suma)
        {
            try
            {
                if (suma <= 0)
                {
                    return BadRequest(new { Mesaj = "Suma trebuie să fie mai mare decât zero." });
                }

                var rezervare = await _serviciuHotel.GetRezervareByIdAsync(rezervareId);
                if (rezervare == null)
                {
                    return BadRequest(new { Mesaj = $"Rezervarea cu ID-ul {rezervareId} nu există." });
                }

                if (rezervare.SumaRamasaDePlata < suma)
                {
                    return BadRequest(new { Mesaj = "Suma introdusă depășește suma rămasă de plată." });
                }

                var clientSecret = await _serviciuPlata.ProceseazaPlataAsync(rezervareId, suma, "RON", "Plată rezervare");
                return Ok(new
                {
                    Mesaj = "Plata a fost inițiată cu succes.",
                    ClientSecret = clientSecret
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }

        // Endpoint pentru refund
        [HttpPost("Refund")]
        public async Task<IActionResult> Refund(string paymentIntentId, decimal? suma)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentIntentId))
                {
                    return BadRequest(new { Mesaj = "ID-ul intenției de plată este necesar." });
                }

                var status = await _serviciuPlata.ProceseazaRefundAsync(paymentIntentId, suma);
                return Ok(new
                {
                    Mesaj = "Refund procesat cu succes.",
                    Status = status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }

        // Endpoint pentru obținerea tuturor rezervărilor
        [HttpGet("GetAllRezervari")]
        public async Task<IActionResult> GetAllRezervari()
        {
            try
            {
                var rezervari = await _serviciuHotel.GetAllRezervariAsync();
                return Ok(rezervari);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }

        // Endpoint pentru obținerea rezervărilor neexpirate
        [HttpGet("GetNonExpiredRezervari")]
        public async Task<IActionResult> GetNonExpiredRezervari()
        {
            try
            {
                var rezervari = await _serviciuHotel.GetNonExpiredRezervari();
                return Ok(rezervari);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mesaj = ex.Message });
            }
        }
    }
}
