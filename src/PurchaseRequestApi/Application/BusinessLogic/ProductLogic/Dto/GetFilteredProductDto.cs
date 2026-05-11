namespace Application.BusinessLogic.ProductLogic.Dto
{
    public class GetFilteredProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> RequestTypeIds { get; set; } = new List<Guid>();
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string? UnitsOfMeasure { get; set; }
    }
}