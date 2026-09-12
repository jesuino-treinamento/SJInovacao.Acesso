namespace SJInovacao.Acesso.Common.Security.Context
{
    public class UserContext : IUserContext
    {
        public string? UserId { get; private set; }
        public string? UserName { get; private set; }
        public string? UserRole { get; private set; }
        public IReadOnlyList<string> Permissions { get; private set; } = Array.Empty<string>();

        public void SetUserData(string? userId, string? userName, string? userRole, IEnumerable<string> permissions)
        {
            UserId = userId;
            UserName = userName;
            UserRole = userRole;
            Permissions = (permissions ?? Enumerable.Empty<string>()).ToList().AsReadOnly();
        }
    }
}