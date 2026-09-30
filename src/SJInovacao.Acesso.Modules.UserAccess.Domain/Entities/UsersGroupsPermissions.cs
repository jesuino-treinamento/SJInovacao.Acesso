namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class UsersGroupsPermissions
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegações
        public User User { get; set; } = null!;
        public GroupPermission Group { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }

}
