using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.UpdateUserPermissionStatus
{
    public class UpdateUserPermissionStatusCommand : IRequest<UpdateUserPermissionStatusResult>
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
