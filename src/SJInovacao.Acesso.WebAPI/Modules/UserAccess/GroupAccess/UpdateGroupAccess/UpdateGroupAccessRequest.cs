namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.UpdateGroupAccess
{
    public class UpdateGroupAccessRequest
    {
        public Guid Id { get; internal set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<Guid>? PermissionIds { get; set; }
    }
}
