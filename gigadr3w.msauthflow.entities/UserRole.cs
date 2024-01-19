namespace gigadr3w.msauthflow.entities
{
    public class UserRole
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public Role Role { get; set; }
        public User User { get; set; }
    }
}
