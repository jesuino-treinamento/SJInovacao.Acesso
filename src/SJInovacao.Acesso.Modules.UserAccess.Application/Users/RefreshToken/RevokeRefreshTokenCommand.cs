using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken
{
    public class RevokeRefreshTokenCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}