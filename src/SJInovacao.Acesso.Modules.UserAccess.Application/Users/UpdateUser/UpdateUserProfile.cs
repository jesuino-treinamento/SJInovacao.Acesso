using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserProfile : Profile
    {
        public UpdateUserProfile()
        {
            CreateMap<DocumentResult, Document>()
               .ConstructUsing(src => new Document(src.Number, src.PersonType));

            //CreateMap<GeolocationResult, Geolocation>()
            //.ConstructUsing(g => new Geolocation(g.Lat, g.Long));

            // Geolocalização
            CreateMap<GeolocationResult, Geolocation>()
                .ConstructUsing(g => new Geolocation(g.Lat, g.Long));

            // Endereço
            CreateMap<AddressDto, AddressCommand>();

            // Documento
            CreateMap<DocumentResult, Document>()
                .ConstructUsing(d => new Document(d.Number, d.PersonType));

            // Nome
            CreateMap<NameResult, Name>()
                .ConstructUsing(n => new Name(n.FirstName, n.LastName));

            // Telefone
            CreateMap<PhoneDto, PhoneCommand>();
            CreateMap<UpdateUserCommand, User>()
                   .ForMember(dest => dest.Password, opt => opt.Ignore())
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                   .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserResult>();
            CreateMap<Name, NameResult>();
            CreateMap<Address, AddressDto>();
            CreateMap<Geolocation, GeolocationResult>();
        }
    }
}
