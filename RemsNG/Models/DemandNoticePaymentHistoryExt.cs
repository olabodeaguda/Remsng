using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DemandNoticePaymentHistoryExt
    {
        public Guid id { get; set; }
        public Guid ownerId { get; set; }
        public string billingNumber { get; set; }
        public decimal amount { get; set; }
        public decimal charges { get; set; }
        public string paymentMode { get; set; }
        public string referenceNumber { get; set; }
        public Guid bankId { get; set; }
        public string paymentStatus { get; set; }
        public int totalSize { get; set; }
        public int billingYear { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
