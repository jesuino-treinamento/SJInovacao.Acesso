using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class GroupPermission : BaseEntity
    {
        public GroupPermission() { } // construtor sem parâmetros para EF

        public GroupPermission(string? name, string? description)
        {
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
                
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Usuários que pertencem ao grupo
        public ICollection<User> Users { get; set; } = new List<User>();

        // Permissões atribuídas ao grupo
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        //public ICollection<Groups_Permissions> Groups_Permissions { get; set; } = new List<Groups_Permissions>();

        public void AddPermission(Permission permission)
        {
            if (!Permissions.Contains(permission))
            {
                Permissions.Add(permission);
            }
        }

        public void UpdateDetails(string name, string description, bool isActive)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
