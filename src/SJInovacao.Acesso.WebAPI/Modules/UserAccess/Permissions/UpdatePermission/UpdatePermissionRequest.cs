using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.UpdatePermission
{
    public class UpdatePermissionRequest
    {
        public Guid Id { get; internal set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
