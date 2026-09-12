namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class AuthenticateDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
