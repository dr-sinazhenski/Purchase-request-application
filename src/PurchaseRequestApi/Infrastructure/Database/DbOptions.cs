using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Database
{
    public sealed class DbOptions
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
