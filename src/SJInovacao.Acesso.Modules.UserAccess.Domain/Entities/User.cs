using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class User : Person, IUser
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = null!;                  // E-mail da pessoa
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public StatusTypes Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        string IUser.Id => Id.ToString();
        string IUser.Username => Username;
        string IUser.Role => Role.ToString();
        public ICollection<Customer> Customers { get; set; } = null!;
        public ICollection<Employee> Employees { get; set; } = null!;
        public ICollection<Supplier> Suppliers { get; set; } = null!;

        // Relacionamento com permissões diretas
        //public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        // Relacionamento com grupos
        //public ICollection<GroupPermission> Groups { get; set; } = new List<GroupPermission>();

        // Implementação da interface
        IEnumerable<string> IUser.Permissions =>
            // Permissões diretas do usuário
            UserPermissions.Select(p => p.PermissionId.ToString())//.Name)
            // + permissões de todos os grupos que o usuário pertence
            .Concat(UserGroups.SelectMany(g => g.Group.Permissions).Select(p => p.Name))
            // evitar duplicatas
            .Distinct();

        IEnumerable<string> IUser.Groups =>
              UserGroups.Select(g => g.Group.Name)
              .Distinct();

        public DateTime? RefreshTokenExpiry { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<GroupsPermissions> GroupsPermissions { get; set; } = new List<GroupsPermissions>();
        public ICollection<UsersGroupsPermissions> UsersGroupsPermissions { get; set; } = new List<UsersGroupsPermissions>();

        public User() { }

        public User(string username, string email, string passwordHash, UserRole role, Name name, Document document)
        : base(name, document) // ou Logical, conforme regra
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Role = role;
            Document = document ?? throw new ArgumentNullException(nameof(document));
            this.Name = name;
            Status = StatusTypes.Active;
            CreatedAt = DateTime.UtcNow;
        }

        // Método para adicionar permissão direta ao usuário
        public void AddPermission(UserPermission userPermission)
        {
            if (!UserPermissions.Any(up => up.PermissionId == userPermission.PermissionId))
            {
                UserPermissions.Add(userPermission);
            }
        }

        // Método para adicionar grupo ao usuário
        public void AddGroup(UserGroup userGroup)
        {
            if (!UserGroups.Any(ug => ug.GroupId == userGroup.GroupId))
            {
                UserGroups.Add(userGroup);
            }
        }

        public void Deactivate()
        {
            Status = StatusTypes.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
