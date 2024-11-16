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

        [HttpGet]
        [Route("GetAllHotels/{filtruNume?}")]
        public async Task<ResponseDto> GetAllHotels(string? filtruNume)
        {
            try
            {
               _responseSMR.Result = await _hotelService.GetAllHotels(filtruNume);
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

        [HttpGet]
        [Route("GetAllHotelsTipCamere")]
        public async Task<ResponseDto> GetAllHotelsTipCamere()
        {
            try
            {
                _responseSMR.Result = await _hotelService.GetAllHotelsTipCamera();
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

        [HttpGet]
        [Route("GetHotelsTipCamere/{filtruNumeHotel?}/{capacitatePersoane?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamere(string? filtruNumeHotel="", int? capacitatePersoane=0)
        {
            try
            {
                _responseSMR.Result = await _hotelService.GetAllHotelsTipCameraFiltered(filtruNumeHotel, capacitatePersoane);
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

        [HttpGet]
        [Route("GetAllHotelsTipCamerePret")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret()
        {
            try
            {
                _responseSMR.Result = await _hotelService.GetAllHotelsTipCameraPret();
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

        [HttpGet]
        [Route("GetHotelsTipCamerePret/{filtruNumeHotel?}/{capacitatePersoane?}/{pretCamera?}")]
        public async Task<ResponseDto> GetAllHotelsTipCamerePret(string? filtruNumeHotel = "", int? capacitatePersoane = 0, 
            float? pretCamera=0)
        {
            try
            {
                _responseSMR.Result = await _hotelService.GetAllHotelsTipCameraPretFiltered(filtruNumeHotel, capacitatePersoane, pretCamera);
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

        [HttpGet]
        [Route("GetAllHotelsWithRatings/{rating?}")]
        public async Task<ResponseDto> GetAllHotelsWithRatings(double? rating)
        {
            try
            {
                _responseSMR.Result = await _hotelService.GetAllHotelsByRating(rating);
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
