using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.CreateGroupUsersPermissions
{
    public class CreateGroupUsersPermissionsResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty; // Novo campo
        public List<PermissionResponse> Permissions { get; set; } = new List<PermissionResponse>();
    }
}
