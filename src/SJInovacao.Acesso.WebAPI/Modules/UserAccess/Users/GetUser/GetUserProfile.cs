using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using SJInovacao.Acesso.WebAPI.Common.Request;
using SJInovacao.Acesso.WebAPI.Common.Response;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.GetUser
{
    public class GetUserProfile : Profile
    {
        public GetUserProfile()
        {
            // Subcomponentes devem vir antes
            CreateMap<NameRequest, Name>()
                    .ConstructUsing(src => new Name(src.FirstName, src.LastName));

            CreateMap<DocumentRequest, Document>()
                .ConstructUsing(src => new Document(src.Number, src.PersonType));

            CreateMap<AddressRequest, AddressCommand>();
            CreateMap<GeolocationRequest, GeolocationCommand>();
            CreateMap<PhoneRequest, PhoneCommand>();            

            // ENTITY → RESULT
            CreateMap<User, CreateUserResult>();
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
