using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticeTaxpayersDetail : AbstractModel
    {
        public Guid id { get; set; }
        public Guid dnId { get; set; }
        public Guid taxpayerId { get; set; }
        public string taxpayersName { get; set; }
        public string billingNumber { get; set; }
        public string addressName { get; set; }
        public string wardName { get; set; }
        public string lcdaName { get; set; }
        public int billingYr { get; set; }
    }
}
