using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    //public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly IPasswordHasher _passwordHasher;
    //    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    //    public AuthenticateUserHandler(
    //        IUserRepository userRepository,
    //        IPasswordHasher passwordHasher,
    //        IJwtTokenGenerator jwtTokenGenerator)
    //    {
    //        _userRepository = userRepository;
    //        _passwordHasher = passwordHasher;
    //        _jwtTokenGenerator = jwtTokenGenerator;
    //    }

    //    public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    //    {
    //        // Busca o usuário com permissões diretas e grupos (com suas permissões)
    //        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

    //        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
    //        {
    //            throw new UnauthorizedAccessException("Invalid credentials");
    //        }

    //        var activeUserSpec = new ActiveUserSpecification();
    //        if (!activeUserSpec.IsSatisfiedBy(user))
    //            throw new UnauthorizedAccessException("User is not active");

    //        var permissions = await _userRepository.GetGroupToUserAsync(user.Id, cancellationToken);

    //        List<string> permissionNames = permissions?.UserGroups?.Where(p => p.IsActive == true).Select(p => p.Group.Name).ToList() ?? new List<string?>();

    //        // 1. Permissões via entidade UserPermission
    //        var directPermissions = permissions?.UserPermissions.Where(p => p.IsActive == true).Select(p => p.Permission.Name).ToList() ?? new List<string>();

    //        // 2. Permissões dos grupos
    //        var groupInfos = new List<UserGroupInfo>();
    //        var allGroupPermissions = new List<string>();           

    //        if (permissions?.UserGroups != null)
    //        {
    //            foreach (var group in permissions.UserGroups.Where(p => p.IsActive == true))
    //            {
    //                var groupPerms = group.Group.UsersGroupsPermissions?.Where(x => x.IsActive == true).Select(p => p.Permission.Name).ToList() ?? new List<string>();
    //                groupInfos.Add(new UserGroupInfo
    //                {
    //                    GroupName = group.Group.Name,
    //                    Permissions = groupPerms
    //                });
    //                allGroupPermissions.AddRange(groupPerms);
    //            }
    //        }

    //        var allGroupGroups = permissions?.UserGroups.Select(p => p.Group.Name).ToList() ?? new List<string?>();

    //        // 3. Permissões totais (união)
    //        var allPermissions = directPermissions.Union(allGroupPermissions).Distinct().ToList();

    //        // 4. Gera token JWT com todas as permissões
    //        // Gera access token (JWT)
    //        var accessToken = _jwtTokenGenerator.GenerateToken(user, allPermissions, allGroupGroups);

    //        // Gera refresh token (GUID)
    //        var refreshToken = Guid.NewGuid().ToString();
    //        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 dias de validade

    //        // Armazena refresh token no banco
    //        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, cancellationToken);

    //        return new AuthenticateUserResult
    //        {
    //            AccessToken = accessToken,
    //            RefreshToken = refreshToken,
    //            Id = user.Id,
    //            Email = user.Email,
    //            Name = user.Username,
    //            Role = user.Role.ToString(),
    //            Permissions = allPermissions,
    //            Groups = groupInfos                    
    //        };
    //    }
    //}

    public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthenticateUserHandler> _logger;

        public AuthenticateUserHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthenticateUserHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<AuthenticateUserResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Tentativa de autenticação para usuário {Email}", request.Email);

                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("Usuário {Email} não encontrado", request.Email);
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
                {
                    _logger.LogWarning("Senha inválida para usuário {Email}", request.Email);
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                var activeUserSpec = new ActiveUserSpecification();
                if (!activeUserSpec.IsSatisfiedBy(user))
                {
                    _logger.LogWarning("Usuário {Email} está inativo ou suspenso", request.Email);
                    throw new UnauthorizedAccessException("User is not active");
                }

                _logger.LogInformation("Usuário {Email} validado com sucesso, carregando permissões", request.Email);

                var permissions = await _userRepository.GetGroupToUserAsync(user.Id, cancellationToken);

                var directPermissions = permissions?.UserPermissions
                    .Where(p => p.IsActive)
                    .Select(p => p.Permission.Name)
                    .ToList() ?? new List<string>();

                var groupInfos = new List<UserGroupInfo>();
                var allGroupPermissions = new List<string>();

                if (permissions?.UserGroups != null)
                {
                    foreach (var group in permissions.UserGroups.Where(p => p.IsActive))
                    {
                        var groupPerms = group.Group.UsersGroupsPermissions?
                            .Where(x => x.IsActive)
                            .Select(p => p.Permission.Name)
                            .ToList() ?? new List<string>();

                        groupInfos.Add(new UserGroupInfo
                        {
                            GroupName = group.Group.Name,
                            Permissions = groupPerms
                        });

                        allGroupPermissions.AddRange(groupPerms);
                    }
                }

                var allPermissions = directPermissions.Union(allGroupPermissions).Distinct().ToList();
                var allGroupNames = permissions?.UserGroups.Select(p => p.Group.Name).ToList() ?? new List<string>();

                _logger.LogInformation("Usuário {Email} possui {PermissionCount} permissões e {GroupCount} grupos",
                    request.Email, allPermissions.Count, allGroupNames.Count);

                var accessToken = _jwtTokenGenerator.GenerateToken(user, allPermissions, allGroupNames);
                var refreshToken = Guid.NewGuid().ToString();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry, cancellationToken);

                _logger.LogInformation("Tokens gerados com sucesso para usuário {Email}", request.Email);

                return new AuthenticateUserResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Username,
                    Role = user.Role.ToString(),
                    Permissions = allPermissions,
                    Groups = groupInfos
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Falha de autenticação para usuário {Email}", request.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado durante autenticação do usuário {Email}", request.Email);
                throw;
            }
        }
    }
}