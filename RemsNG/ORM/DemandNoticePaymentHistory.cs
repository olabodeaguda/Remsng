using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.ORM
{
    public class DemandNoticePaymentHistory : AbstractModel
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
        public string bankName { get; set; }
        public bool IsWaiver { get; set; }
        [NotMapped]
        public decimal TotalBillAmount { get; set; }
    }
}
