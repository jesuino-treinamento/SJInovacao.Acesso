namespace  SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
