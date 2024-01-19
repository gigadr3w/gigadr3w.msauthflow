using System.ComponentModel.DataAnnotations;

namespace gigadr3w.msauthflow.autenticator.api.Requests
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "Field 'Email' is mandatory")]
        [EmailAddress(ErrorMessage = "Field 'Email' must be a valid mail address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Field 'Password' is mandatory")]
        public string Password { get; set; }
    }
}
