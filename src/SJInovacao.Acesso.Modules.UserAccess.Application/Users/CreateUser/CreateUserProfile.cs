//using AutoMapper;
//using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
//using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
//using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
//using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

//namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser
//{
//    public class CreateUserProfile : Profile
//    {
//        public CreateUserProfile()
//        {
//            CreateMap<CreateUserCommand, Person>()
//                .ForMember(dest => dest.Name,
//                    opt => opt.MapFrom(src => new Name(src.Name.FirstName, src.Name.LastName)))
//                .ForMember(dest => dest.Document,
//                    opt => opt.MapFrom(src => new Document(src.Document.Number, src.Document.PersonType)));

//            CreateMap<CreateUserCommand, User>()
//                .ForMember(dest => dest.p,
//                    opt => opt.MapFrom(src => src))
//                .ForMember(dest => dest.Password, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

//            COMMAND → ENTITY
//            CreateMap<CreateUserCommand, User>()
//                .ForMember(dest => dest.Password, opt => opt.Ignore())
//                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
//                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

//            CreateMap<CreateUserCommand, Person>()
//                .ForMember(dest => dest.Document,
//                    opt => opt.MapFrom(src => new Document(src.Document.Number, src.Document.PersonType)))
//                .ForMember(dest => dest.Name,
//                    opt => opt.MapFrom(src => new Name(src.Name.FirstName, src.Name.LastName)));


//            ENTITY → RESULT
//            CreateMap<User, CreateUserResult>();
//            CreateMap<Name, NameResult>();
//            CreateMap<Document, DocumentResult>();
//            CreateMap<Address, AddressResult>();
//            CreateMap<Phone, PhoneResult>();
//            CreateMap<Geolocation, GeolocationResult>();

//            RESULT → COMMAND(Se necessário)
//            CreateMap<DocumentResult, Document>()
//                .ConstructUsing(src => new Document(src.Number, src.PersonType));

//            CreateMap<GeolocationResult, Geolocation>()
//                .ConstructUsing(g => new Geolocation(g.Lat, g.Long));

//            CreateMap<AddressResult, AddressCommand>();
//            CreateMap<NameResult, Name>()
//                .ConstructUsing(n => new Name(n.FirstName, n.LastName));
//            CreateMap<PhoneResult, PhoneCommand>();
//        }
//    }
//}

using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.CreateUser;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

public class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        // WEB REQUEST → COMMAND
        CreateMap<User, CreateUserCommand>();
        CreateMap<Name, NameCommand>();
        CreateMap<Document, DocumentCommand>();
        CreateMap<Address, AddressCommand>();
        CreateMap<Phone, PhoneCommand>();

        // COMMAND → DOMAIN ENTITIES
        CreateMap<CreateUserCommand, Person>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => new Name(src.Name.FirstName, src.Name.LastName)))
            .ForMember(dest => dest.Document,
                opt => opt.MapFrom(src => new Document(src.Document.Number, src.Document.PersonType)));

        CreateMap<CreateUserCommand, User>();
           

        // DOMAIN → RESULT
        CreateMap<User, CreateUserResult>();
        CreateMap<Person, PersonDto>();
        CreateMap<Name, NameResult>();
        CreateMap<Document, DocumentResult>();
        CreateMap<Address, AddressDto>();
        CreateMap<Phone, PhoneDto>();
    }
}
