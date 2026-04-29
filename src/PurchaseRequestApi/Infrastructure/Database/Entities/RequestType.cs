using System;

namespace Infrastructure.Database.Entities
{
    public class RequestType : Entity
    {
        required public string Name { get; set; }

        public ICollection<Request> Requests { get; set; }
        public ICollection<Product> Product { get; set; }
    }
}