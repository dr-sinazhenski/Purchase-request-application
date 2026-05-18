using System;

namespace Infrastructure.Database.Entities
{
    public class Role : Entity
    {
        required public string Name { get; set; }

        public ICollection<Account> Account { get; set; }
    }
}