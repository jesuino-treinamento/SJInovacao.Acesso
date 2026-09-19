using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus
{
    public class UpdateGroupPermissionStatusCommand : IRequest<UpdateGroupPermissionStatusResult>
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
