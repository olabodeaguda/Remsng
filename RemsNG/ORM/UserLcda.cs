using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class UserLcda
    {
        public Guid userId { get; set; }
        public Guid lgdaId { get; set; }

        public User user { get; set; }
        public Lgda lcda { get; set; }


        public virtual ICollection<Role> roles { get; set; }
    }
}
