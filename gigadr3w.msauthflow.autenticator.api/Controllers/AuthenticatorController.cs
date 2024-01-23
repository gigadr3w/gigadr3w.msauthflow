using gigadr3w.msauthflow.autenticator.api.Filters.Attributes;
using gigadr3w.msauthflow.autenticator.api.Requests;
using gigadr3w.msauthflow.autenticator.api.Responses;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;

namespace gigadr3w.msauthflow.autenticator.api.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticatorController : Controller
    {
        private readonly ILogger<AuthenticatorController> _logger;
        private readonly IAuthenticatorService _autehticator;

        public AuthenticatorController(IAuthenticatorService autehticator,
                                        ILogger<AuthenticatorController> logger)
            => (_autehticator, _logger) = (autehticator, logger);

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
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
