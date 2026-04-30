using System;

namespace Infrastructure.Database.Entities
{
    public class Comment : Entity
    {
        public Guid RequestId { get; set; }
        public Guid? AccountId { get; set; }
        required public string Text { get; set; }
        public DateTime CreationTime { get; set; }

        public Request Request { get; set; }
        public Account? Account { get; set; }
    }
}