using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DN_ArrearsModel
    {
        public string billingNo { get; set; }
        public Guid taxpayerId { get; set; }
        public int billingYr { get; set; }
        public int previousBillingYr { get; set; }
        public string arrearstatus { get; set; }
        public string createdBy { get; set; }
        public Guid dnId { get; set; }
    }
}
