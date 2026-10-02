using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GroupUsersPermissionResult
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; } = string.Empty; // Novo campo

        //public List<Guid>? PermissionIds { get; set; }
        //public string PermissionName { get; set; } = string.Empty; // Novo campo
        //public bool PermissionIsActive { get; set; } = true;

        public bool UserIsActive { get; set; } = true;
        public List<PermissionDto?>? Permissions { get; set; } = null!;

        
    }
}
