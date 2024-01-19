namespace gigadr3w.msauthflow.authenticator.iterator.Models
{
    public class UserModel
    {
        public string Email { get;  set; }
        public string Password { get; set; }
        public bool IsAuthorized { get; set; }
        public string UnhautorizedMessage { get; set; }
        public ICollection<RoleModel> Roles { get; set; }
    }
}
