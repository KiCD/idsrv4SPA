using TB.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Migrations.Data
{
    public class UserCreationInfo
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string Password { get; set; }
    }
}
