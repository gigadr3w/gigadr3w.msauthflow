using gigadr3w.msauthflow.autenticator.api.Requests;
using gigadr3w.msauthflow.authenticator.iterator.Models;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gigadr3w.msauthflow.autenticator.api.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticatorController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAuthenticatorService _autehticator;

        public AuthenticatorController(IAuthenticatorService autehticator,
                                        ILogger logger)
            => (_autehticator, _logger) = (autehticator, logger);

        [AllowAnonymous]
        [Route("Authenticate")]
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
