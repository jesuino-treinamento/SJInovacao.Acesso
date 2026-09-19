namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class UserPermissionsDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<PermissionDto> Permissions { get; set; }
        
    }
}
