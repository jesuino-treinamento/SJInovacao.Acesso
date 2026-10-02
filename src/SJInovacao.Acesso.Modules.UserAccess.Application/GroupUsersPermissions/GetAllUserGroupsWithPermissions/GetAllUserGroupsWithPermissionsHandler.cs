using MediatR;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUserGroupsWithPermissionsHandler
        : IRequestHandler<GetAllUserGroupsWithPermissionsQuery, IEnumerable<GroupUsersPermissionDto>>
    {
        private readonly IGroupPermissionUserRepository _repository;

        public GetAllUserGroupsWithPermissionsHandler(IGroupPermissionUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GroupUsersPermissionDto>> Handle(GetAllUserGroupsWithPermissionsQuery query, CancellationToken ct)
        {
            var queryable = _repository.Query();

            if (query.OnlyActiveUsers)
                queryable = queryable.Where(g => !g.Group.UsersGroupsPermissions.Any(ugp => !ugp.IsActive));

            if (!string.IsNullOrWhiteSpace(query.PermissionNameFilter))
                queryable = queryable.Where(g => g.Group.UsersGroupsPermissions
                    .Any(p => EF.Functions.ILike(p.Permission.Name, $"%{query.PermissionNameFilter}%")));

            if (query.MaxResults.HasValue)
                queryable = queryable.Take(query.MaxResults.Value);

            return await queryable
                .Select(g => new GroupUsersPermissionDto
                {
                    UserId = g.UserId,
                    UserName = g.User.Name.ToString(),
                    GroupId = g.Group.Id,
                    GroupName = g.Group.Name,
                    Permissions = g.Group.UsersGroupsPermissions
                        .Select(p => new PermissionDto
                        {
                            Id = p.Permission.Id,
                            Name = p.Permission.Name,
                            Description = p.Permission.Description,
                            IsActive = p.IsActive,
                            CreatedAt = p.CreatedAt,
                            UpdatedAt = p.UpdatedAt
                        }).ToList()
                })
                .ToListAsync(ct);
        }



    }
}
