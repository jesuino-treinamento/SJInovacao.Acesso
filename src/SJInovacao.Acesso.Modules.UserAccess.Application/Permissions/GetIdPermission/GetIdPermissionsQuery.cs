using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.GetIdPermission
{
    public class GetIdPermissionsQuery : IRequest<PermissionDto>
    {
        public Guid PermissionId { get; set; }
    }
}
