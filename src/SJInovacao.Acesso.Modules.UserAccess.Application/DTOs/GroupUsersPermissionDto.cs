namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class GroupUsersPermissionDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty; // Novo campo

        public bool UserIsActive { get; internal set; } = true;
        public List<PermissionDto?> Permissions { get; set; } = new List<PermissionDto?>();
    }
}
