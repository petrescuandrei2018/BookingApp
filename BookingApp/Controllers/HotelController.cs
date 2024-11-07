using AutoMapper;
using BookingApp.Data;
using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private ResponseDto _responseSMR;
        private IHotelService _hotelService;

        public HotelController(AppDbContext database, IMapper mapper, IHotelService hotelService)
        {
            _responseSMR = new ResponseDto();
            _hotelService = hotelService;
        }

        [HttpGet]
        [Route("GetAllHotels")]
        public async Task<ResponseDto> GetAllHotels()
        {
            try
            {
               _responseSMR.Result = await _hotelService.GetAllHotels();
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
    }
}
