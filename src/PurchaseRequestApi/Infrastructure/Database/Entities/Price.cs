using System;

namespace Infrastructure.Database.Entities
{
    public class Price
    {
        public Guid ProductId { get; set; }
        public Guid RegionId { get; set; }
        public decimal Amount { get; set; }
        required public string UnitsOfMeasure { get; set; }

        required public Product Product { get; set; }
        required public Region Region { get; set; }
    }
}