using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class RoleExtension
    { 
        public Guid id { get; set; }
        public string roleName { get; set; }
        public Guid domainId { get; set; }
        public string roleStatus { get; set; }
        public string domainName { get; set; }
    }
}
