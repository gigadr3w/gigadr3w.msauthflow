using gigadr3w.msauthflow.autenticator.api.Filters.Attributes;
using gigadr3w.msauthflow.autenticator.api.Requests;
using gigadr3w.msauthflow.autenticator.api.Responses;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace gigadr3w.msauthflow.autenticator.api.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticatorController : Controller
    {
        private readonly ILogger<AuthenticatorController> _logger;
        private readonly ILoginService _autehticator;
        private readonly IJwtTokenService _jwtTokenService;


        public AuthenticatorController(ILoginService autehticator,
                                        IJwtTokenService jwtTokenService,
                                        ILogger<AuthenticatorController> logger)
            => (_autehticator, _jwtTokenService, _logger) = (autehticator, jwtTokenService, logger);

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        [SwaggerOperation(
            Summary = "Credential validation",
            Description = "An anomymous request that authenticate the user and return its JWT",
            OperationId = "CredentialsAuthentication",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(201, "Valid credentials", typeof(AuthenticateResponse))]
        [SwaggerResponse(401, "Invalid credentials")]
        [ThrottleMaxEmail(30, 3)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            _logger.LogInformation("Authentication request required");

            UserModel userModel = await _autehticator.Authenticate(new UserModel { Email = request.Email, Password = request.Password });

            if (userModel.IsAuthorized)
            {
                List<Claim> claims = new() { new Claim(ClaimTypes.Email, userModel.Email) };
                claims.AddRange(userModel.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

                JwtGenerationModel jwtTokenModel = new()
                {
                    Claims = claims,
                    ExpirationTime = TimeSpan.FromDays(1)
                };

                string jwt = _jwtTokenService.GenerateToken(jwtTokenModel);

                return Ok(new AuthenticateResponse { ApiKey = jwt, Email = request.Email });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
