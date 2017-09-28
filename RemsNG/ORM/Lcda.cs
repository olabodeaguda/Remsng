using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Lgda : AbstractModel
    {
        public Guid id { get; set; }
        public Guid domainId { get; set; }
        public string lcdaName { get; set; }
        public string lcdaCode { get; set; }
        public Guid addressId { get; set; }
        public string lcdaStatus { get; set; }
    }
}
