using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticeItemPenalty:AbstractModel
    {
        public Guid id { get; set; }
        public string billingNo { get; set; }
        public Guid taxpayerId { get; set; }
        public decimal totalAmount { get; set; }
        public decimal amountPaid { get; set; }
        public Guid itemId { get; set; }
        public int originatedYear { get; set; }// billing year arrears
        public int billingYr { get; set; }
        public string itemPenaltyStatus { get; set; }
    }
}
