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
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        // Relacionamento com grupos
        public ICollection<GroupPermission> Groups { get; set; } = new List<GroupPermission>();

        // Implementação da interface
        IEnumerable<string> IUser.Permissions =>
            // Permissões diretas do usuário
            Permissions.Select(p => p.Name)
            // + permissões de todos os grupos que o usuário pertence
            .Concat(Groups.SelectMany(g => g.Permissions).Select(p => p.Name))
            // evitar duplicatas
            .Distinct();

        public DateTime? RefreshTokenExpiry { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

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
        public void AddPermission(Permission permission)
        {
            if (!Permissions.Contains(permission))
            {
                Permissions.Add(permission);
            }
        }

        // Método para adicionar grupo ao usuário
        public void AddGroup(GroupPermission group)
        {
            if (!Groups.Contains(group))
            {
                Groups.Add(group);
            }
        }

        public void Deactivate()
        {
            Status = StatusTypes.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
