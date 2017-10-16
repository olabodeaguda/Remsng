using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Role
    {
        public Guid id { get; set; }
        public string roleName { get; set; }

        [ForeignKey("lgda")]
        public Nullable<Guid> domainId { get; set; }
        public string roleStatus { get; set; }

        public virtual Lgda lgda { get; set; }
    }
}
