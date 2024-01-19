using System.Security.Claims;

namespace gigadr3w.msauthflow.authenticator.iterator.Models
{
    public class JwtTokenModel
    {
        public List<Claim> Claims { get; set; }

        public TimeSpan ExpirationTime { get; set; }
    }
}
