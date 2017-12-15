using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DNAmountDueModel
    {
        public Guid id { get; set; }
        public decimal itemAmount { get; set; }
        public decimal amountPaid { get; set; }
        public string itemDescription { get; set; }
        public string category { get; set; }
    }
}
