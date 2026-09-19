namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus
{
    public class UpdateGroupPermissionStatusResult
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
