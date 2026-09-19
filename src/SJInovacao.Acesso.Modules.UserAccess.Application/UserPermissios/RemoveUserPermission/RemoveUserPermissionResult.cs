namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.RemoveUserPermission
{
    public class RemoveUserPermissionResult
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime UpdatedAt { get; set; }
    }
}
