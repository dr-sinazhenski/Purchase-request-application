namespace Application.BusinessLogic.AccountLogic.Dto
{
    public class LoginResDto
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }
}