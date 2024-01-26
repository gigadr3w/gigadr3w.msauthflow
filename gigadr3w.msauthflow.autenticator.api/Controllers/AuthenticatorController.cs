using gigadr3w.msauthflow.autenticator.api.Filters.Attributes;
using gigadr3w.msauthflow.autenticator.api.Requests;
using gigadr3w.msauthflow.autenticator.api.Responses;
using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Filters;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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

            UserModel userModel = await _autehticator.Authenticate(request.Email, request.Password);

            if (userModel.IsAuthorized)
            {
                List<Claim> claims = new List<Claim>(userModel.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)))
                {
                    new Claim("UserId", userModel.Id.ToString())
                };

                string jwt = await _jwtTokenService.GenerateToken(userModel.Email, claims);

                return Ok(new AuthenticateResponse { ApiKey = jwt, Email = request.Email });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete]
        [Route("Dismiss")]
        [Authorize(AuthenticationSchemes = JwtTokenConfiguration.DEFAULT_SCHEMA)]
        [SwaggerOperation(
            Summary = "Credential dismiss",
            Description = "An authorized request that dismiss the current user authentication",
            OperationId = "CredentialsDismiss",
            Tags = new[] { "Authentication" }
        )]
        [SwaggerResponse(204, "Valid credentials", typeof(AuthenticateResponse))]
        [SwaggerResponse(401, "Invalid credentials")]
        [SwaggerOperationFilter(typeof(SwaggerAuthenticationFilter))]
        public async Task<IActionResult> Dismiss()
        {
            await _jwtTokenService.DismissTokenFor(User);
            return StatusCode(204);
        }
    }
}
