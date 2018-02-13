using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DnError
    {
        public Guid id { get; set; }
        public string batchno { get; set; }
        public string errorType { get; set; }
        public string errorvalue { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
