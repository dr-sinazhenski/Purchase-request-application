using System;

namespace Infrastructure.Database.Entities
{
    public class Region : Entity
    {
        required public string Name { get; set; }
        required public string Currency { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Price> Prices { get; set; }
    }
}