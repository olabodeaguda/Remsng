using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Ward
    {
        public Guid id { get; set; }
        public string wardName { get; set; }
        public Guid lcdaId { get; set; }
    }
}
