using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    public class AuthenticateUserCommand : IRequest<AuthenticateUserResult>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
