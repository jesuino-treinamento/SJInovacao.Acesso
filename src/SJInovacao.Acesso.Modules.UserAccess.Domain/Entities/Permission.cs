using SJInovacao.Acesso.Common.Auditing;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Permission : BaseEntity, IDeactivatable, IAuditable
    {
        public Permission() { }

        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

        // Grupos que possuem esta permissão
        public ICollection<GroupsPermissions> GroupsPermissions { get; set; } = new List<GroupsPermissions>();
        public ICollection<UsersGroupsPermissions> UsersGroupsPermissions { get; set; } = new List<UsersGroupsPermissions>();

       public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

    }
}
