using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<GeolocationResult, Geolocation>()
            .ConstructUsing(src => new Geolocation(src.Lat, src.Long));

            CreateMap<AddressCommand, Address>()
                .ForCtorParam("geolocation", opt =>
                    opt.MapFrom(src => src.Geolocation));
        }
    }
}
