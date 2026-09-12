using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GetUser
{
    public class GetUserProfile : Profile
    {
        public GetUserProfile()
        {
            CreateMap<User, UserResult>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameResult
                 {
                     FirstName = src.Name.FirstName,
                     LastName = src.Name.LastName
                 }))
                 .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
                 .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));

            CreateMap<Address, AddressDto>();
            CreateMap<Geolocation, GeolocationResult>();
            CreateMap<Phone, PhoneDto>(); // ✅ Aqui
        }
    }
}
