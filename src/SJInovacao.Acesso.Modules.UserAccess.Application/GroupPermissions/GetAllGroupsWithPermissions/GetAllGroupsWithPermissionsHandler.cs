using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions
{
    public class GetAllGroupsWithPermissionsHandler
        : IRequestHandler<GetAllGroupsWithPermissionsQuery, IEnumerable<GroupPermissionsDto>>
    {
        private readonly IGroupPermissionRepository _repository;

        public GetAllGroupsWithPermissionsHandler(IGroupPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GroupPermissionsDto>> Handle(GetAllGroupsWithPermissionsQuery query, CancellationToken ct)
        {
            var groups = await _repository.GetAllWithGroupPermissionsAsync(ct);

            // Filtro: apenas grupos ativos
            if (query.OnlyActiveUsers)
                groups = groups.Where(g => g.IsActive);

            // Filtro: por nome de permissão
            if (!string.IsNullOrWhiteSpace(query.PermissionNameFilter))
                groups = groups.Where(g => g.Permissions
                    .Any(p => p.Name.Contains(query.PermissionNameFilter)));

            // Limite de resultados
            if (query.MaxResults.HasValue)
                groups = groups.Take(query.MaxResults.Value);

            // Monta DTO
            var result = groups.Select(g => new GroupPermissionsDto
            {
                Id = g.Id,
                Name = g.Name,
                Permissions = g.Permissions
                    .Where(p => p.IsActive) // só permissões ativas
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToList()
            });

            return result;
        }

    }
}
