namespace Application.BusinessLogic.RequestLogic.Dto
{
    public class GetRequestsFilteredDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RequesterId { get; set; }
    }
}