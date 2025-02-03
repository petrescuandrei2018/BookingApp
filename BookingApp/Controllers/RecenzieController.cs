using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecenzieController : ControllerBase
    {
        private readonly IRecenzieService _recenzieService;

        public RecenzieController(IRecenzieService recenzieService)
        {
            _recenzieService = recenzieService;
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> ObtineRecenzii(int hotelId)
        {
            var recenzii = await _recenzieService.ObtineRecenziiHotel(hotelId);
            return Ok(recenzii);
        }

        [HttpPost]
        public async Task<IActionResult> AdaugaRecenzie([FromBody] RecenzieDto recenzieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _recenzieService.AdaugaRecenzie(recenzieDto);
            return Ok(new { Mesaj = "Recenzie adăugată cu succes!" });
        }

        [HttpDelete("{recenzieId}")]
        public async Task<IActionResult> StergeRecenzie(int recenzieId)
        {
            await _recenzieService.StergeRecenzie(recenzieId);
            return Ok(new { Mesaj = "Recenzie ștearsă cu succes!" });
        }
    }
}
