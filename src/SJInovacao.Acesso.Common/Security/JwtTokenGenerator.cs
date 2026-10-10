using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SJInovacao.Acesso.Common.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(IUser user, IEnumerable<string> permissions, IEnumerable<string> groups)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.Role, user.Role)
            };

            claims.AddRange(permissions.Select(permission =>
                new Claim("permissions", permission)));

            claims.AddRange(groups.Select(group =>
               new Claim("groups", group)));            

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
