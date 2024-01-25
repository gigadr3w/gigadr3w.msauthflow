using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface IJwtTokenService
    {
        public string GenerateToken(JwtGenerationModel model);
        public string Save(JwtModel token);
        public JwtModel GetOrThrow(string token);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtTokenConfiguration _configuration;

        public JwtTokenService(JwtTokenConfiguration configuration)
            => _configuration = configuration;

        public string GenerateToken(JwtGenerationModel model)
        {

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(_configuration.SecretKey));
            SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha256);

            DateTime expiration = DateTime.Now.AddMilliseconds(model.ExpirationTime.TotalMilliseconds);

            JwtSecurityToken token = new (
                claims: model.Claims,
                expires: expiration, 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token); 

        }

        public JwtModel GetOrThrow(string token)
        {
            throw new NotImplementedException();
        }

        public string Save(JwtModel token)
        {
            throw new NotImplementedException();
        }
    }
}
