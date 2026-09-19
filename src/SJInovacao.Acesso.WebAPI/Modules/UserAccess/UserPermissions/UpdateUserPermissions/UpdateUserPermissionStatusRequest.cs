namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions.UpdateUserPermissions
{
    public class RemoveUserPermissionRequest
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
