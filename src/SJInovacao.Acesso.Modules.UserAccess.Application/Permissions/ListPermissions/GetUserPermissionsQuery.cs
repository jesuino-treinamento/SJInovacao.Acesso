using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions
{
    public class GetUserPermissionsQuery : IRequest<List<PermissionDto>>
    {
        public Guid UserId { get; set; }
    }
}
