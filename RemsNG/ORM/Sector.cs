using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Sector : AbstractModel
    {
        public Guid id { get; set; }
        public string sectorName { get; set; }
    }
}
