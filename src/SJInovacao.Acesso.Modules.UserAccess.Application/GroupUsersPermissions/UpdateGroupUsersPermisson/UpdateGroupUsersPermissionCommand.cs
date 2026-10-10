using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission
{
    public class UpdateGroupUsersPermissionCommand : IRequest<GroupUsersPermissionDto>
    {
        public Guid UserId { get; set; }
        public Guid GroupAccessId { get; set; }
        public bool UserIsActive { get; set; }
        public List<Guid> PermissionIds { get; set; } = null!;
        //public string? PermissionName { get; set; } = null; // Novo campo
        public bool? PermissionIsActive { get; set; } 
    }
}
