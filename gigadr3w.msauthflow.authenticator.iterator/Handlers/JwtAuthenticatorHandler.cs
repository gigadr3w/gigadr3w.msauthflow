using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace gigadr3w.msauthflow.authenticator.iterator.Handlers
{
    /// <summary>
    /// Jwt ApiKey Authenticator Handler (for each consumer api service)
    /// </summary>
    public class JwtAuthenticatorHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;

        /// <summary>
        /// Authenticate the current http request with api key token.
        /// </summary>
        /// <param name="apiKeyHeader">Label for the ApiKey Header</param>
        /// <param name="secretKey">Symmetric encryption private key</param>
        public JwtAuthenticatorHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration) : base(options, logger, encoder, clock)
        => (_jwtTokenConfiguration) = (jwtTokenConfiguration.Value);

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.TryGetValue(_jwtTokenConfiguration.ApiKeyHeader, out StringValues apiKeyHeaderValues))
            {
                string encryptedToken = apiKeyHeaderValues.FirstOrDefault();

                SymmetricSecurityKey issuerSigningKey = new (Encoding.UTF8.GetBytes(_jwtTokenConfiguration.SecretKey));

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
                    return Task.FromResult(AuthenticateResult.Fail($"Bad formatted token"));
                }

                try
                {
                    handler.ValidateToken(encryptedToken, validationParameters, out var validatedToken);
                    var token = validatedToken as JwtSecurityToken;

                    if (token != null)
                    {
                        ClaimsIdentity identity = new(token.Claims, Scheme.Name);
                        ClaimsPrincipal principal = new(identity);
                        AuthenticationTicket ticket = new(principal, Scheme.Name);

                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                    else
                    {
                        return Task.FromResult(AuthenticateResult.Fail($"Invalid token"));
                    }
                }
                catch (SecurityTokenExpiredException)
                {
                    return Task.FromResult(AuthenticateResult.Fail($"Expired token"));
                }
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail($"{_jwtTokenConfiguration.ApiKeyHeader} header not found"));
            }
        }
    }
}
