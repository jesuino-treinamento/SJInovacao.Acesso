using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission
{
    public class RemoveGroupPermissionCommand : IRequest<RemoveGroupPermissionResult>
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
