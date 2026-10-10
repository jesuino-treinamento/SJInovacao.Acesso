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
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var permissions = await _userRepository.GetGroupToUserAsync(user.Id, cancellationToken);

            List<string> permissionNames = permissions?.UserGroups?.Where(p => p.IsActive == true).Select(p => p.Group.Name).ToList() ?? new List<string>();

            var directPermissions = user.UserPermissions?
                .Where(up => up.IsActive)
                .Select(up => up.Permission.Name)
                .ToList() ?? new List<string>();


            var groupInfos = new List<UserGroupInfo>();
            var allGroupPermissions = new List<string>();

            if (permissions?.UserGroups != null)
            {
                foreach (var group in user.UserGroups.Where(p => p.IsActive == true))
                {
                    var groupPerms = group.Group.UsersGroupsPermissions?.Where(x => x.IsActive == true).Select(p => p.Permission.Name).ToList() ?? new List<string>();
                    groupInfos.Add(new UserGroupInfo
                    {
                        GroupName = group.Group.Name,
                        Permissions = groupPerms
                    });
                    allGroupPermissions.AddRange(groupPerms);
                }
            }

            var allPermissions = permissionNames.Union(allGroupPermissions).Distinct().ToList();
            var allGroupGroups = permissions?.UserGroups.Select(p => p.Group.Name).ToList() ?? new List<string>();

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user, allPermissions, allGroupGroups);

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
                Permissions = allPermissions,          
                Groups = groupInfos                    
            };
        }
    }
}