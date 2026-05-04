namespace Application.BusinessLogic.PriceLogic.Dto
{
    public class CrudPriceDto
    {
        public Guid ProductId { get; set; }
        public Guid RegionId { get; set; }
        public decimal Amount { get; set; }
        public string UnitsOfMeasure { get; set; }
    }
}