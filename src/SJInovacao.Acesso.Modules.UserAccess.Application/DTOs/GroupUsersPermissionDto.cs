namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class GroupUsersPermissionDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; } = string.Empty; 
        public bool PermissionIsActive { get; set; } = true;
        public bool UserIsActive { get; set; } = true;
        public List<PermissionDto> Permissions { get; set; } = null!;
        
    }
}
