using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class GroupsPermissions
    {
        public Guid GroupId { get; set; }
        public GroupPermission Group { get; set; } = new();

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = new();

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

