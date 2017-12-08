using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticePaymentHistory:AbstractModel
    {
        public Guid id { get; set; }
        public Guid ownerId { get; set; }
        public string billingNumber { get; set; }
        public decimal amount { get; set; }
        public string paymentMode { get; set; }
        public string referenceNumber { get; set; }
        public Guid bankId { get; set; }
    }
}
