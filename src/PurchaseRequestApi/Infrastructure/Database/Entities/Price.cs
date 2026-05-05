using System;

namespace Infrastructure.Database.Entities
{
    public class Price
    {
        public Guid ProductId { get; set; }
        public Guid RegionId { get; set; }
        public decimal Amount { get; set; }
        required public string UnitsOfMeasure { get; set; }

        public Product Product { get; set; }
        public Region Region { get; set; }
    }
}