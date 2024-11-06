using AutoMapper;
using BookingApp.Data;
using BookingApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private ResponseDto _responseSMR;
        private readonly AppDbContext _database;
        private IMapper _mapper;

        public HotelController(AppDbContext database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _responseSMR = new ResponseDto();
        }

        [HttpGet]
        [Route("GetAllHotels")]
        public ResponseDto GetAllHotels()
        {
            try
            {
                var listHotels = _database.Hotels.ToList();
                _responseSMR.Result = _mapper.Map<List<ResponseHotelDto>>(listHotels);
                return _responseSMR;
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
