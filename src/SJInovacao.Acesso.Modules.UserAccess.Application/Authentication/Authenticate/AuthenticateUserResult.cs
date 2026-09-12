using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authentication.Authenticate
{
    //public class AuthenticateUserResult
    //{
    //    public AuthenticateUserResult(string authenticationError)
    //    {
    //        IsAuthenticated = false;
    //        AuthenticationError = authenticationError;
    //    }

    //    public AuthenticateUserResult(UserDto user)
    //    {
    //        this.IsAuthenticated = true;
    //        this.User = user;
    //    }

    //    public bool IsAuthenticated { get; }

    //    public string AuthenticationError { get; }

    //    public UserDto User { get;  }
    //}

    public sealed class AuthenticateUserResult
    {
        // public string Token { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();           // Todas as permissões (diretas + grupos)
        public List<UserGroupInfo> Groups { get; set; } = new List<UserGroupInfo>();        // Detalhamento dos grupos
    }

    public class UserGroupInfo
    {
        public string GroupName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
