namespace SJInovacao.Acesso.Common.Security
{
    public interface IUser
    {
        public string Id { get; }
        public string Username { get; }
        public string Role { get; }
        IEnumerable<string> Permissions { get; }
        IEnumerable<string> Groups { get; }
    }
}
