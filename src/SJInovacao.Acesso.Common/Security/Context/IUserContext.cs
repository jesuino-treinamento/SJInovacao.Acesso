namespace SJInovacao.Acesso.Common.Security.Context
{
    public interface IUserContext
    {
        string? UserId { get; }
        string? UserName { get; }
        string? UserRole { get; }
        IReadOnlyList<string> Permissions { get; }
    }
}
