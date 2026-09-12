using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken
{
    public class RefreshTokenCommand : IRequest<AuthenticateUserResult>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}