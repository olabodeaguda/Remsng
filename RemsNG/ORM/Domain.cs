using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Domain
    {
        public Guid id { get; set; }
        public string domainName { get; set; }
        public string domainCode { get; set; }
        public DateTime datecreated { get; set; }
        public string domainStatus { get; set; }
        public Guid addressId { get; set; }
        public string domainType { get; set; }
    }
}
