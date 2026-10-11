using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUserGroupsWithPermissionsQuery : IRequest<IEnumerable<GroupUsersPermissionDto>>
    {
        public bool OnlyActiveUsers { get; set; } = false;
        public string? PermissionNameFilter { get; set; }
        public int? MaxResults { get; set; }
    }
}
