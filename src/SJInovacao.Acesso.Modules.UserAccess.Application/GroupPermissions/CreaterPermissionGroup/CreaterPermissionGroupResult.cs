namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreaterPermissionGroup
{
    public class CreaterPermissionGroupResult
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
