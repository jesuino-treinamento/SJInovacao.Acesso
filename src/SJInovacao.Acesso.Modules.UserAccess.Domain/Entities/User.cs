using SJInovacao.Acesso.Common.Auditing;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class User : Person, IUser, IAuditable
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public StatusTypes Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        string IUser.Id => Id.ToString();
        string IUser.Username => Username;
        string IUser.Role => Role.ToString();
        IEnumerable<string> IUser.Permissions =>
            UserPermissions.Select(p => p.Permission.Name)
                .Concat(UserGroups.SelectMany(g => g.Group.Permissions).Select(p => p.Name))
                .Distinct();
        IEnumerable<string> IUser.Groups =>
            UserGroups.Select(g => g.Group.Name)
                .Distinct();
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<UsersGroupsPermissions> UsersGroupsPermissions { get; set; } = new List<UsersGroupsPermissions>();


        public User() { }

        public User(string username, string email, string passwordHash, UserRole role, Name name, Document document)
        : base(name, document)
            {
                Username = username ?? throw new ArgumentNullException(nameof(username));
                Email = email ?? throw new ArgumentNullException(nameof(email));
                Password = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
                Role = role;
                Status = StatusTypes.Active;
                CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            Status = StatusTypes.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
