namespace  SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature
{
    /// <summary>
    /// Represents the authentication request model for user login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user's email address for authentication.
        /// Must be a valid email format.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password for authentication.
        /// Must match the stored password after hashing.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
