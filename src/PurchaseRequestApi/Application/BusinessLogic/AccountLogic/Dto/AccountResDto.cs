namespace Application.BusinessLogic.AccountLogic.Dto
{
    public class AccountResDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public Guid RegionId { get; set; }
        public string RegionName { get; set; }
        public Guid? ApproverProfileId { get; set; }
        public string? ApproverProfileName { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
        public List<string> RoleNames { get; set; } = new();
    }
}