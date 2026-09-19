namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions.UpdateGroupPermissions
{
    public class RemoveGroupPermissionRequest
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
