using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions
{
    public class GetAllGroupsWithPermissionsQuery : IRequest<IEnumerable<GroupPermissionsDto>>
    {
        public bool OnlyActiveUsers { get; set; } = false;
        public string? PermissionNameFilter { get; set; }
        public int? MaxResults { get; set; }
    }
}
