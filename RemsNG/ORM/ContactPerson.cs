using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class ContactPerson : AbstractModel
    {
        public Guid id { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Guid taxPayerId { get; set; }
    }
}
