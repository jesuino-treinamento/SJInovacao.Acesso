namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.UpdateUserPermissionStatus
{
    public class UpdateUserPermissionStatusResult
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
