namespace gigadr3w.msauthflow.entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EnabledService { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
