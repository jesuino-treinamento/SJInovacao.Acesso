using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetAllUsersWithPermissions
{
    public class GetAllUsersWithPermissionsQuery : IRequest<IEnumerable<UserPermissionsDto>>
    {
        public bool OnlyActiveUsers { get; set; } = false;
        public string? PermissionNameFilter { get; set; }
        public int? MaxResults { get; set; }
    }
}
