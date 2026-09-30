using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreaterPermissionGroup
{ 
    public class CreaterPermissionGroupCommand : IRequest<CreaterPermissionGroupResult>
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
