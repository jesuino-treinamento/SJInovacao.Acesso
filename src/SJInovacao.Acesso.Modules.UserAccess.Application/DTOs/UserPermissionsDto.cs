namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class UserPermissionsDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new ();        
    }
}
