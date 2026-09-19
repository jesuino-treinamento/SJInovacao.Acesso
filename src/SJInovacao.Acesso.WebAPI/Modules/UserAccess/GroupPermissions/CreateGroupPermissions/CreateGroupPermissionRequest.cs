namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.CreateGroupPermission
{
    public class CreateGroupPermissionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid>? PermissionIds { get; set; }
    }
}
