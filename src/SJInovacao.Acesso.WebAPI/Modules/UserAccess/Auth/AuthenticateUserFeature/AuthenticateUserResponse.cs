using System;

namespace  SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature
{
    /// <summary>
    /// Represents the response returned after user authentication
    /// </summary>
    public sealed class AuthenticateUserResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } // segundos

        /// <summary>
        /// Gets or sets the user's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's role in the system
        /// </summary>
        public string Role { get; set; } = string.Empty;

        // public List<string> GroupUsers { get; set; } = new();

        public List<string> Permissions { get; set; }= new();           // Todas as permissões (diretas + grupos)
        public List<UserGroupInfo> Groups { get; set; }= new();         // Detalhamento dos grupos
    }

    public class UserGroupInfo
    {
        public string GroupName { get; set; }= string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
