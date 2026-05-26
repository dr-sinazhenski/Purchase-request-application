using Infrastructure.Database;
using Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
    public static class DBSeeder
    {
        public static RequestType SeedRequestType(AppDbContext db, Guid? id = null, string name = "Default Type")
        {
            var requestType = new RequestType
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };

            db.RequestTypes.Add(requestType);
            db.SaveChanges();

            return requestType;
        }
    }
}
