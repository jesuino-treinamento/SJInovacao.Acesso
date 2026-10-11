using SJInovacao.Acesso.Modules.UserAccess.Application.Users;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class UserGroupDTO
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public UserResult UserDTO { get; set; } = null!;
        public GroupPermissionsDto GroupDTO { get; set; } = null!;
    }
}
