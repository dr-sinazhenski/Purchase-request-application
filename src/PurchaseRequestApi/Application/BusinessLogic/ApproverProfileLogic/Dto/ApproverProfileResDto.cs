namespace Application.BusinessLogic.ApproverProfileLogic.Dto
{
    public class ApproverProfileResDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }
}