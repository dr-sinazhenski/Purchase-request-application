using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
