namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.UpdateGroupAccess
{
    public class UpdateGroupAccessResult
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
