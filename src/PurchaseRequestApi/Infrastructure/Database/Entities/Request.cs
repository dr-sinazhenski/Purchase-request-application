namespace Infrastructure.Database.Entities
{
    public class Request : Entity
    {
        required public string Title { get; set; }
        required public string Description { get; set; }
        required public RequestStatus Status { get; set; }
        required public DateTime CreatedAt { get; set; }
        required public DateTime EditedAt { get; set; }


        //public Guid RequesterId { get; set; }
        //public Account Requester { get; set; }
        public Guid RequestTypeId { get; set; }
        required public RequestType RequestType { get; set; }
        

        //public ICollection<Comment> Comments { get; set; }
        public ICollection<RequesterProduct> RequesterProducts { get; set; }
    }
}