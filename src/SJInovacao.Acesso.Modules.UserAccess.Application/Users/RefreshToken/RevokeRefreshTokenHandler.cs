using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RevokeRefreshTokenHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.RevokeRefreshTokenAsync(request.UserId, cancellationToken);
            return true;
        }
    }
}