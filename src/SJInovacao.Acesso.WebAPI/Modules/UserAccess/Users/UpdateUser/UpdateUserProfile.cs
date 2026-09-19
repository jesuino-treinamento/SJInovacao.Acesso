using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using SJInovacao.Acesso.WebAPI.Common.Request;
using SJInovacao.Acesso.WebAPI.Common.Response;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.UpdateUser
{
    public class UpdateUserProfile : Profile
    {
        public UpdateUserProfile()
        {
            // // Request API -> Command Application
             CreateMap<UserRequest, UpdateUserCommand>();

            // // Value Objects (WebApi Models -> Domain Value Objects)
            // CreateMap<NameRequest, Name>()
            //     .ConstructUsing(x => new Name(x.FirstName, x.LastName));

            // CreateMap<DocumentRequest, Document>()
            //     .ConstructUsing(x => new Document(x.Number, x.PersonType));

            // // AddressModel (WebApi) -> CreateAddressCommand (Application)
            // CreateMap<AddressRequest, AddressCommand>()
            //     .ForMember(dest => dest.Id, opt => opt.Ignore())
            //     //.ForMember(dest => dest.AddressId, opt => opt.Ignore())
            //     .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            //     .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            //     .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Neighborhood))
            //     .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            //     .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            //     .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
            //     .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));

            // // GeolocationModel WebApi -> GeolocationModel Application
            //// CreateMap<GeolocationRequest, Geolocation>();

            // // PhoneModel (WebApi) -> CreatePhoneCommand (Application)
            // CreateMap<PhoneRequest, PhoneCommand>();

            // // Domain Entity User -> Application Result
            // CreateMap<User, UserResult>()
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //     .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document))
            //     .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            //     .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));

            // // Domain Value Objects -> Result Models
            // CreateMap<Name, NameResult>();
            // CreateMap<Document, DocumentResult>();
            // CreateMap<Address, AddressResult>();
            // CreateMap<Phone, PhoneResult>();

            // // Application Result -> API Response
            // CreateMap<UserResult, UserResponse>()
            //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameRequest
            //     {
            //         FirstName = src.Name.FirstName,
            //         LastName = src.Name.LastName
            //     }))
            //     .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document))
            //     .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            //     .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));

            // // Result Models -> Response Models (para coleções internas)
            // CreateMap<NameResult, Name>();
            // CreateMap<DocumentResult, Document>();
            // CreateMap<AddressResult, Address>();
            // CreateMap<PhoneResult, Phone>();

            // // Mapeamento para Geolocation (domínio -> result)
            // CreateMap<Geolocation, GeolocationResult>();

            // // Mapeamento para Geolocation (result -> response)
            // CreateMap<GeolocationResult, Geolocation>();

            // Subcomponentes devem vir antes
            CreateMap<NameRequest, Name>()
                    .ConstructUsing(src => new Name(src.FirstName, src.LastName));

            CreateMap<DocumentRequest, Document>()
                .ConstructUsing(src => new Document(src.Number, src.PersonType));

            CreateMap<AddressRequest, AddressCommand>();
            CreateMap<GeolocationRequest, GeolocationCommand>();
            CreateMap<PhoneRequest, PhoneCommand>();

            // ENTITY → RESULT
            CreateMap<User, UserResult>();
            CreateMap<Name, NameResult>();
            CreateMap<Document, DocumentResult>();
            CreateMap<Address, AddressDto>();
            CreateMap<Phone, PhoneDto>();
            CreateMap<Geolocation, GeolocationResult>();

            // RESULT → RESPONSE
            CreateMap<NameResult, NameResponse>();
            CreateMap<DocumentResult, DocumentResponse>();
            CreateMap<PhoneDto, PhoneResponse>();
            CreateMap<GeolocationResult, GeolocationResponse>();
            CreateMap<AddressDto, AddressResponse>();

            CreateMap<UserResult, UserResponse>();
        }
    }
}
