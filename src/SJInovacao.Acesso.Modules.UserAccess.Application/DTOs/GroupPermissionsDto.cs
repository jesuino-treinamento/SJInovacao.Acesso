namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class GroupPermissionsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
