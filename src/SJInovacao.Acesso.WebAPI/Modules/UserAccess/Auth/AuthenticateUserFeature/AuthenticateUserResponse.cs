namespace  SJInovacao.Acesso.WebAPI.Modules.UserAccess.Auth.AuthenticateUserFeature
{
    public sealed class AuthenticateUserResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; }= new();           
        public List<UserGroupInfo> Groups { get; set; }= new();         
    }

    public class UserGroupInfo
    {
        public string GroupName { get; set; }= string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
