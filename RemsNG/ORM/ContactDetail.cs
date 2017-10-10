using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class ContactDetail : AbstractModel
    {
        public Guid id { get; set; }
        public Guid ownerId { get; set; }
        public string contactValue { get; set; }
        public string contactType { get; set; }

    }
}
