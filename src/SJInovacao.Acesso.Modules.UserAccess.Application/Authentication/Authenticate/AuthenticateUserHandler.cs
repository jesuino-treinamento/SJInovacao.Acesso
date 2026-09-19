using MediatR;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticateUserHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            // Busca o usuário com permissões diretas e grupos (com suas permissões)
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var activeUserSpec = new ActiveUserSpecification();
            if (!activeUserSpec.IsSatisfiedBy(user))
                throw new UnauthorizedAccessException("User is not active");

            var permissions = await _userRepository.GetGroupToUserAsync(user.Id, cancellationToken);

           // var _perm = permissions?.UserPermissions.Select(p => p.Permission.Name).ToList() ?? new List<string>();

            List<string> permissionNames = permissions?.UserGroups?.Select(p => p.Group.Name).ToList() ?? new List<string?>();

            // 1. Permissões via entidade UserPermission
            var directPermissions = permissions?.UserPermissions.Select(p => p.Permission.Name).ToList() ?? new List<string>();


            // 2. Permissões dos grupos
            var groupInfos = new List<UserGroupInfo>();
            var allGroupPermissions = new List<string>();           

            if (permissions?.UserGroups != null)
            {
                foreach (var group in permissions.UserGroups)
                {
                    var groupPerms = group.Group.Permissions?.Select(p => p.Name).ToList() ?? new List<string>();
                    groupInfos.Add(new UserGroupInfo
                    {
                        GroupName = group.Group.Name,
                        Permissions = groupPerms
                    });
                    allGroupPermissions.AddRange(groupPerms);
                }
            }

            var allGroupGroups = permissions?.UserGroups.Select(p => p.Group.Name).ToList() ?? new List<string?>();

            // 3. Permissões totais (união)
            var allPermissions = directPermissions.Union(allGroupPermissions).Distinct().ToList();

            // 4. Gera token JWT com todas as permissões
            // Gera access token (JWT)
            var accessToken = _jwtTokenGenerator.GenerateToken(user, allPermissions, allGroupGroups);

            // Gera refresh token (GUID)
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 dias de validade

            // Armazena refresh token no banco
            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, cancellationToken);

            return new AuthenticateUserResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Id = user.Id,
                Email = user.Email,
                Name = user.Username,
                Role = user.Role.ToString(),
                Permissions = allPermissions,// allPermissions,          // lista plana
                Groups = groupInfos                    // detalhamento
            };
        }
    }
}