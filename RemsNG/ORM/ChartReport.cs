using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class ChartReport
    {
        public Guid id { get; set; }
        public string wardName { get; set; }
        public decimal itemAmount { get; set; }
        public decimal amountPaid { get; set; }
    }
}
