using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class GroupsPermissions
    {
        public Guid GroupId { get; set; }
        public GroupPermission? Group { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

