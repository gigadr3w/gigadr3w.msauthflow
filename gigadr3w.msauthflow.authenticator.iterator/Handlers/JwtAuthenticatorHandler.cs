using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;

namespace gigadr3w.msauthflow.authenticator.iterator.Handlers
{
    /// <summary>
    /// Jwt ApiKey Authenticator Handler (for each consumer api service)
    /// </summary>
    public class JwtAuthenticatorHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;
        private readonly IJwtTokenService _jwtTokenService;
        /// <summary>
        /// Authenticate the current http request with api key token.
        /// </summary>
        /// <param name="apiKeyHeader">Label for the ApiKey Header</param>
        /// <param name="secretKey">Symmetric encryption private key</param>
        public JwtAuthenticatorHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IJwtTokenService jwtTokenService,
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration) : base(options, logger, encoder, clock)
        => (_jwtTokenService, _jwtTokenConfiguration) = (jwtTokenService, jwtTokenConfiguration.Value);

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.TryGetValue(_jwtTokenConfiguration.ApiKeyHeader, out StringValues apiKeyHeaderValues))
            {
                string encryptedToken = apiKeyHeaderValues.FirstOrDefault() ?? string.Empty;

                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    return await _jwtTokenService.Authenticate(encryptedToken, Scheme.Name);
                }
            }
            return AuthenticateResult.Fail($"{_jwtTokenConfiguration.ApiKeyHeader} header not found");
            
        }
    }
}
