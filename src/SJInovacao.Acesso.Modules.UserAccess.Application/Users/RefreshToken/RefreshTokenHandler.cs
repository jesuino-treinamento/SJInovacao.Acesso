using MediatR;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RefreshTokenHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticateUserResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Busca o usuário pelo refresh token (válido e não expirado)
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            // Verifica expiração do refresh token (se você armazenar data)
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var permissions = await _userRepository.GetGroupToUserAsync(user.Id, cancellationToken);

            List<string> permissionNames = permissions?.Groups.Select(p => p.Name).ToList() ?? new List<string>();

            // 1. Permissões diretas do usuário
            var directPermissions = user.Permissions?.Select(p => p.Name).ToList() ?? new List<string>();

            // 2. Permissões dos grupos
            var groupInfos = new List<UserGroupInfo>();
            var allGroupPermissions = new List<string>();

            if (permissions?.Groups != null)
            {
                foreach (var group in user.Groups)
                {
                    var groupPerms = group.Permissions?.Select(p => p.Name).ToList() ?? new List<string>();
                    groupInfos.Add(new UserGroupInfo
                    {
                        GroupName = group.Name,
                        Permissions = groupPerms
                    });
                    allGroupPermissions.AddRange(groupPerms);
                }
            }

            // 3. Permissões totais (união)
            var allPermissions = permissionNames.Union(allGroupPermissions).Distinct().ToList();

            // Gera novo access token
            var newAccessToken = _jwtTokenGenerator.GenerateToken(user, allPermissions);

            // Opcional: gera novo refresh token (rotacionar)
            var newRefreshToken = Guid.NewGuid().ToString();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, newRefreshTokenExpiry, cancellationToken);

            return new AuthenticateUserResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Id = user.Id,
                Email = user.Email,
                Name = user.Username,
                Role = user.Role.ToString(),
                Permissions = allPermissions,          // lista plana
                Groups = groupInfos                    // detalhamento
            };
        }
    }
}