
using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate;

namespace  SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature
{
    public sealed class AuthenticateUserProfile : Profile
    {
        public AuthenticateUserProfile()
        {
            CreateMap<LoginRequest, AuthenticateUserCommand>();
            CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();
        }
    }
}
