using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetUserPermissions
{
    public class GetUserPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
    {
        public Guid UserId { get; set; }
    }
}

