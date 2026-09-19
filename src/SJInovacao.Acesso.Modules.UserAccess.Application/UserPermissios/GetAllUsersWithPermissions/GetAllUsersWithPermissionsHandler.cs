using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetAllUsersWithPermissions
{
    public class GetAllUsersWithPermissionsHandler
        : IRequestHandler<GetAllUsersWithPermissionsQuery, IEnumerable<UserPermissionsDto>>
    {
        private readonly IUserPermissionRepository _repository;

        public GetAllUsersWithPermissionsHandler(IUserPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserPermissionsDto>> Handle(GetAllUsersWithPermissionsQuery query, CancellationToken ct)
        {
            var users = await _repository.GetAllWithPermissionsAsync(ct);

            // Filtro: apenas usuários ativos
            if (query.OnlyActiveUsers)
                users = users.Where(u => u.Status == StatusTypes.Active);

            // Filtro: por nome de permissão
            if (!string.IsNullOrWhiteSpace(query.PermissionNameFilter))
                users = users.Where(u => u.UserPermissions
                    .Any(up => up.Permission.Name.Contains(query.PermissionNameFilter)));

            // Limite de resultados
            if (query.MaxResults.HasValue)
                users = users.Take(query.MaxResults.Value);

            // Monta DTO
            var result = users.Select(u => new UserPermissionsDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Permissions = u.UserPermissions
                    .Where(up => up.IsActive) // só permissões ativas
                    .Select(up => new PermissionDto
                    {
                        Id = up.Permission.Id,
                        Name = up.Permission.Name,
                        Description = up.Permission.Description,
                        IsActive = up.IsActive,
                        CreatedAt = up.CreatedAt,
                        UpdatedAt = up.UpdatedAt
                    })
                    .ToList()
            });

            return result;
        }
    }
}
