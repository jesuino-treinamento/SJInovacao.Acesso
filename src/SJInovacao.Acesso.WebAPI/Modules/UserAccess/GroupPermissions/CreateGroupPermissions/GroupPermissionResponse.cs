using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Features.GroupPermissions.CreateGroupPermission
{
    public class GroupPermissionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PermissionResponse> Permissions { get; set; } = new();
    }
}
