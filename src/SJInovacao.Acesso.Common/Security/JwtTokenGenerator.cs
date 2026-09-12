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

        /// <summary>
        /// Initializes a new instance of the JWT token generator.
        /// </summary>
        /// <param name="configuration">Application configuration containing the necessary keys for token generation.</param>
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for a specific user.
        /// </summary>
        /// <param name="user">User for whom the token will be generated.</param>
        /// <returns>Valid JWT token as string.</returns>
        /// <remarks>
        /// The generated token includes the following claims:
        /// - NameIdentifier (User ID)
        /// - Name (Username)
        /// - Role (User role)
        /// 
        /// The token is valid for 8 hours from the moment of generation.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when user or secret key is not provided.</exception>
        public string GenerateToken(IUser user, IEnumerable<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Name, user.Username),
               new Claim(ClaimTypes.Role, user.Role)
               //new Claim("permissions", string.Join(",", user.Permissions))
            };

            // ✅ Corrigir aqui: criar uma claim por permissão
            //claims.AddRange(user.Permissions.Select(p => new Claim("permissions", p)));
            // Adiciona todas as permissions como claims
            claims.AddRange(permissions.Select(permission =>
                new Claim("permissions", permission)));

            //// Adiciona as permissões como claims individuais
            //foreach (var permission in user.Permissions)
            //{
            //    claims.Add(new Claim("permissions", permission));
            //}

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
