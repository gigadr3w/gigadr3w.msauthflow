using System.Security.Claims;

namespace gigadr3w.msauthflow.authenticator.iterator.Models
{
    public class JwtGenerationModel
    {
        public ICollection<Claim> Claims { get; set; }

        public TimeSpan ExpirationTime { get; set; }
    }
}
