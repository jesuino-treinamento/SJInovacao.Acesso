using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions.CreateUserPermissions
{
    public class UserPermissionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<PermissionResponse?> Permissions { get; set; } = new();
    }
}
