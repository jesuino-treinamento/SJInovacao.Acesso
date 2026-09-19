using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class UserPermission
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

