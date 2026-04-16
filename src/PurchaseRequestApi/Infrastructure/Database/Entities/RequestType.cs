using System;

namespace Infrastructure.Database.Entities
{
    public class RequestType : Entity
    {
        required public string Name { get; set; }

        required public ICollection<Request> Requests { get; set; }
        required public ICollection<Product> Product { get; set; }
    }
}