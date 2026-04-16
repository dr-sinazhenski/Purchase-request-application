using System;

namespace Infrastructure.Database.Entities
{
    public class Comment : Entity
    {
        public Guid RequestId { get; set; }
        public Guid AccountId { get; set; }
        required public string Text { get; set; }
        public DateTime CreationTime { get; set; }

        required public Request Request { get; set; }
        required public Account Account { get; set; }
    }
}