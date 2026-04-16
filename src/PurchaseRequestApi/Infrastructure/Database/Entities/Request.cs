using System;

namespace Infrastructure.Database.Entities
{
    public class Request : Entity
    {
        public Guid RequestTypeId { get; set; }
        public Guid RequesterId { get; set; }
        required public string Status { get; set; }

        required public RequestType RequestType { get; set; }
        required public Account Requester { get; set; }
        required public ICollection<Comment> Comments { get; set; }
        required public ICollection<RequesterProduct> RequesterProducts { get; set; }
    }
}