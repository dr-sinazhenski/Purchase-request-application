using System;

namespace Infrastructure.Database.Entities
{
    public class RequesterProduct 
    {
        public Guid RequestId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        required public Request Request { get; set; }
        required public Product Product { get; set; }
    }
}