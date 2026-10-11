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
        CreateMap<User, CreateUserCommand>();
        CreateMap<Name, NameCommand>();
        CreateMap<Document, DocumentCommand>();
        CreateMap<Address, AddressCommand>();
        CreateMap<Phone, PhoneCommand>();
        CreateMap<CreateUserCommand, Person>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => new Name(src.Name.FirstName, src.Name.LastName)))
            .ForMember(dest => dest.Document,
                opt => opt.MapFrom(src => new Document(src.Document.Number, src.Document.PersonType)));
        CreateMap<CreateUserCommand, User>();
        CreateMap<User, CreateUserResult>();
        CreateMap<Person, PersonDto>();
        CreateMap<Name, NameResult>();
        CreateMap<Document, DocumentResult>();
        CreateMap<Address, AddressDto>();
        CreateMap<Phone, PhoneDto>();
    }
}
