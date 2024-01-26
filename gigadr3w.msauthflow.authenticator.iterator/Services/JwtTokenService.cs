using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.sharedcache.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace gigadr3w.msauthflow.authenticator.iterator.Services
{
    public interface IJwtTokenService
    {
        public Task<string> GenerateToken(string email, List<Claim> claims);
        public Task<string> GenerateToken(string email, List<Claim> claims, TimeSpan validFor);        
        public Task<AuthenticateResult> Authenticate(string encryptedToken, string authenticationType);
        public Task DismissTokenFor(ClaimsPrincipal user);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _cacheKeyPrefix = "rn:authentication-token:{0}";

        private readonly JwtTokenConfiguration _configuration;

        private readonly ISharedCache _sharedCache;

        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(
            IOptions<JwtTokenConfiguration> configuration,
            ISharedCache sharedCache,
            ILogger<JwtTokenService> logger)
            => (_configuration, _sharedCache, _logger) = (configuration.Value, sharedCache, logger);

        public async Task<AuthenticateResult> Authenticate(string encryptedToken, string authenticationType)
        {
            SymmetricSecurityKey issuerSigningKey = new(Encoding.UTF8.GetBytes(_configuration.SecretKey));

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // TODO - check for token validation tolleration
            };

            if (!handler.CanReadToken(encryptedToken))
            {
                return AuthenticateResult.Fail($"Bad formatted token");
            }

            try
            {
                handler.ValidateToken(encryptedToken, validationParameters, out var validatedToken);
                JwtSecurityToken token = validatedToken as JwtSecurityToken;

                Claim? claimEmail = token?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

                if (claimEmail == null) return AuthenticateResult.Fail($"No email found in claims");

                string? cachedToken = await _sharedCache.Get<string>(string.Format(_cacheKeyPrefix, claimEmail.Value));

                if (string.IsNullOrEmpty(cachedToken) || cachedToken != encryptedToken) return AuthenticateResult.Fail("Invalid cache token for current user");

                if (token != null)
                {
                    ClaimsIdentity identity = new(token.Claims, authenticationType);
                    ClaimsPrincipal principal = new(identity);
                    AuthenticationTicket ticket = new(principal, authenticationType);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail($"Invalid token");
                }
            }
            catch (SecurityTokenExpiredException)
            {
                return AuthenticateResult.Fail($"Expired token");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"{ex.Message}", ex);
                return AuthenticateResult.Fail($"Exception handling token");
            }
        }

        public async Task<string> GenerateToken(string email, List<Claim> claims, TimeSpan validFor)
        {
            DateTime expiration = DateTime.Now.Add(validFor);

            if (!claims.Any(c => c.Type == ClaimTypes.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration.SecretKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            string encryptedToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Stores current token for the current user key in the shared cache
            await _sharedCache.Set(string.Format(_cacheKeyPrefix, email), encryptedToken, validFor);

            return encryptedToken;
        }

        public async Task<string> GenerateToken(string email, List<Claim> claims)
        {
            TimeSpan validFor = _configuration.ValidFor;
            return await GenerateToken(email, claims, validFor);
        }

        public async Task DismissTokenFor(ClaimsPrincipal user)
        {
            Claim claimEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?? throw new KeyNotFoundException($"Claims do not contain email!");

            await _sharedCache.Delete(string.Format(_cacheKeyPrefix, claimEmail.Value));
        }
    }
}
