namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.DeleteGroupUsersPermissions
{
    public class DeleteGroupUsersPermissionsRequest
    {
        public Guid UserId { get; internal set; }
        public Guid GroupAccessId { get; internal set; }
        public bool UserIsActive { get; internal set; } = false;
    }
}
