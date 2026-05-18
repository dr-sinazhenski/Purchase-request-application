using System;

namespace Infrastructure.Database.Entities
{
    public class ApproverProfile : Entity
    {
        required public string Name { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}