using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class UserDomain
    {
        public Guid userId { get; set; }
        public Guid domainId { get; set; }

        public User user { get; set; }
        public Domain domain { get; set; }

    }
}
