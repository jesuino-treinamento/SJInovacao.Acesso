using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUserGroupsWithPermissionsHandler
        : IRequestHandler<GetAllUserGroupsWithPermissionsQuery, IEnumerable<GroupUsersPermissionDto>>
    {
        private readonly IGroupPermissionUserRepository _repository;

        public GetAllUserGroupsWithPermissionsHandler(IGroupPermissionUserRepository repository)
            => _repository = repository;

        public async Task<IEnumerable<GroupUsersPermissionDto>> Handle(
            GetAllUserGroupsWithPermissionsQuery query, CancellationToken ct)
        {
            var projections = await _repository.GetAllWithFiltersAsync(
                query.OnlyActiveUsers,
                query.PermissionNameFilter,
                query.MaxResults,
                ct);

            return projections.Select(p => new GroupUsersPermissionDto
            {
                UserId = p.UserId,
                UserName = p.UserName,
                GroupId = p.GroupId,
                GroupName = p.GroupName,
                UserIsActive = p.UserIsActive,
                Permissions = p.Permissions.Select(perm => new PermissionDto
                {
                    Id = perm.Id,
                    Name = perm.Name,
                    Description = perm.Description,
                    IsActive = perm.IsActive,
                    CreatedAt = perm.CreatedAt,
                    UpdatedAt = perm.UpdatedAt
                }).ToList()
            });
        }
    }
}