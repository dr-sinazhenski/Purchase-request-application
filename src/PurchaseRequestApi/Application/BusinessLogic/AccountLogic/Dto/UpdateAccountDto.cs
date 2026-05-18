namespace Application.BusinessLogic.AccountLogic.Dto
{
        public class UpdateAccountDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Guid RegionId { get; set; }
        public Guid? ApproverProfileId { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
    }
}