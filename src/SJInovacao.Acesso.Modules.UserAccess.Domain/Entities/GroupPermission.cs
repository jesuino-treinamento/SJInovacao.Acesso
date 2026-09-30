using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class GroupPermission : BaseEntity
    {
        public GroupPermission() { } 

        public GroupPermission(string name, string description)
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

        // Permissões atribuídas ao grupo
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();       
        public ICollection<GroupsPermissions> GroupsPermissions { get; set; } = new List<GroupsPermissions>();
        public ICollection<UsersGroupsPermissions> UsersGroupsPermissions { get; set; } = new List<UsersGroupsPermissions>();
       
    }

}
