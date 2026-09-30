namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid>? PermissionIds { get; set; }
    }
}
