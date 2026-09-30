namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class UserPermission
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = new();

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = new();

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

