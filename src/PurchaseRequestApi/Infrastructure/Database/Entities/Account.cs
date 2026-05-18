namespace Infrastructure.Database.Entities
{
    public class Account : Entity
    {
        required public string Login { get; set; }
        required public string Password { get; set; }
        required public string Name { get; set; }
        public Guid RegionId { get; set; }
        public Guid? ApproverProfileId { get; set; }

        public Region Region { get; set; }
        public ApproverProfile ApproverProfile { get; set; }
        public ICollection<Role> Role { get; set; }
        //public ICollection<Comment> Comments { get; set; }
        public ICollection<Request> Requests { get; set; }
    }
}