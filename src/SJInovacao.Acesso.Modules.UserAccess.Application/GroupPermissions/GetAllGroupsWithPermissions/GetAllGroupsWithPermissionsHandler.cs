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

            if (query.OnlyActiveUsers)
                groups = groups.Where(g => g.IsActive);

            if (!string.IsNullOrWhiteSpace(query.PermissionNameFilter))
            {
                var filter = query.PermissionNameFilter;
                groups = groups.Where(g => g.Permissions != null
                                        && g.Permissions.Any(p => p.Name.Contains(filter)));
            }

            if (query.MaxResults.HasValue)
                groups = groups.Take(query.MaxResults.Value);

            var result = groups.Select(g => new GroupPermissionsDto
            {
                Id = g.Id,
                Name = g.Name,
                Permissions = g.GroupsPermissions
                    .Select(p => new PermissionDto
                    {
                        Id = p.Permission.Id,
                        Name = p.Permission.Name,
                        Description = p.Permission.Description,
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
