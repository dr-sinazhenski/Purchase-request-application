namespace Application.BusinessLogic.CommentLogic.Dto
{
    public class CrudCommentDto
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public Guid? AccountId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
    }
}