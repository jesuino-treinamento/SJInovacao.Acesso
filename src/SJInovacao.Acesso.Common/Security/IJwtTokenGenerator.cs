namespace SJInovacao.Acesso.Common.Security
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IUser user, IEnumerable<string> permissions);
    }
}
