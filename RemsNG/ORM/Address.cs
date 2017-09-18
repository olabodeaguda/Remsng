using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Address : AbstractModel
    {
        public Guid id { get; set; }
        public string addressnumber { get; set; }
        public Guid streetId { get; set; }


        public Domain Domains { get; set; }
    }
}
