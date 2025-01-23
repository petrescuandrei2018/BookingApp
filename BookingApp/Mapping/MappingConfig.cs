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

                // Mapare completă pentru GetAllRezervariDto
                config.CreateMap<Rezervare, GetAllRezervariDto>()
                    .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.PretCamera.TipCamera.Hotel.Name))
                    .ForMember(dest => dest.NumeCamera, opt => opt.MapFrom(src => src.PretCamera.TipCamera.Name))
                    .ForMember(dest => dest.Pret, opt => opt.MapFrom(src => src.PretCamera.PretNoapte))
                    .ForMember(dest => dest.Stare, opt => opt.MapFrom(src => src.Stare))
                    .ReverseMap();
            });

            return mappingConfig;
        }
    }
}
