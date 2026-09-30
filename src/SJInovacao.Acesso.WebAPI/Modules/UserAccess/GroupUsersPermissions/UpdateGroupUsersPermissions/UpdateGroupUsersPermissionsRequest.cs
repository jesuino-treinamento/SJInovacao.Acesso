namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.UpdateGroupUsersPermissions
{
    public class UpdateGroupUsersPermissionsRequest
    {
        public Guid UserId { get; internal set; }
        public Guid GroupAccessId { get; internal set; }
        public bool UserIsActive { get; internal set; } = true;
        public List<Guid>? PermissionIds { get;  set; } = null;
        public bool? PermissionIsActive { get; set; } = null;
    }
}
