using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{   
    public sealed class AuthenticateUserResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();          
        public List<UserGroupInfo> Groups { get; set; } = new List<UserGroupInfo>();        
    }

    public class UserGroupInfo
    {
        public string GroupName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
