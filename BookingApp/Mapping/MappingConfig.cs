using AutoMapper;
using BookingApp.Models;
using BookingApp.Models.Dtos;

namespace BookingApp.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Hotel, ResponseHotelDto>();
                config.CreateMap<ResponseHotelDto, Hotel>();
                config.CreateMap<RezervareDto, Rezervare>().ReverseMap();
                config.CreateMap<GetAllRezervariDto, Rezervare>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
