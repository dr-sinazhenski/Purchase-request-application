namespace Infrastructure.Database.Entities
{
    public class Account : Entity
    {
        required public string Login { get; set; }
        required public string Password { get; set; }
        required public string Name { get; set; }
        public Guid RegionId { get; set; }
        public Guid? ApproverProfileId { get; set; }

        required public Region Region { get; set; }
        required public ApproverProfile ApproverProfile { get; set; }
        required public ICollection<Role> Role { get; set; }
        required public ICollection<Comment> Comments { get; set; }
        required public ICollection<Request> Requests { get; set; }
    }
}