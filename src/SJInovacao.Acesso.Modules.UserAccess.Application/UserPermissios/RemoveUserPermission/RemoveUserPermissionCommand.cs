using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.RemoveUserPermission
{
    public class RemoveUserPermissionCommand : IRequest<RemoveUserPermissionResult>
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
