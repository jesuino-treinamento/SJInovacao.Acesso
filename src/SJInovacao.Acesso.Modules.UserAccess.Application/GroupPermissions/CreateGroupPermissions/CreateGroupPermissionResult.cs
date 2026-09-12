using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
