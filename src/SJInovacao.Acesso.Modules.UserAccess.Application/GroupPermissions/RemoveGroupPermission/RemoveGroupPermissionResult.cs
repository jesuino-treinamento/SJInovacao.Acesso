namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission
{
    public class RemoveGroupPermissionResult
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime UpdatedAt { get; set; }
    }
}
