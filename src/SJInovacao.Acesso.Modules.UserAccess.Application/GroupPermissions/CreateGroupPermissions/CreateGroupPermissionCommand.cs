using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionCommand : IRequest<CreateGroupPermissionResult>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
