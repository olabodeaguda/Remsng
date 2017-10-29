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
        public decimal amount { get; set; }
        public Guid itemId { get; set; }
        public string status { get; set; }
    }
}
