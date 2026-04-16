using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database.Entities
{
    public class Product : Entity
    {
        required public string Name { get; set; }
        required public string Description { get; set; }

        public ICollection<Price> Prices { get; set; }
        public ICollection<RequestType> RequestType { get; set; }
        public ICollection<RequesterProduct> RequesterProducts { get; set; }
    }
}