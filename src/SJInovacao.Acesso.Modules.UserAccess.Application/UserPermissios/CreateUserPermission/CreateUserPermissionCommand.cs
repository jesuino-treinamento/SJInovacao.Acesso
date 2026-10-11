using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissios.CreateUserPermission
{
    public class CreateUserPermissionCommand : IRequest<CreateUserPermissionResult>
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
