using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface IJwtTokenService
    {
        public string GenerateToken(JwtTokenModel model);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtTokenConfiguration _configuration;

        public JwtTokenService(JwtTokenConfiguration configuration)
            => _configuration = configuration;

        public string GenerateToken(JwtTokenModel model)
        {

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(_configuration.SecretKey));
            SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha256);

            //var claims = new[]
            //{
            //        new Claim(JwtRegisteredClaimNames.Sub, "example-user-id"),
            //        new Claim(JwtRegisteredClaimNames.Email, "example@example.com"),
            //        // Aggiungi eventuali altre claims necessarie
            //};

            DateTime expiration = DateTime.Now.AddMilliseconds(model.ExpirationTime.TotalMilliseconds);

            JwtSecurityToken token = new (
                claims: model.Claims,
                expires: expiration, 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token); ;

        }
    }
}
