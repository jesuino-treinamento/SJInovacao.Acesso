using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    public sealed class AuthenticateUserProfile : Profile
    {
        public AuthenticateUserProfile()
        {
            CreateMap<User, AuthenticateUserResult>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<User, AuthenticateUserCommand>();
            CreateMap<AuthenticateUserResult, User>();
        }
    }
}
