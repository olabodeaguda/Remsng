using System;

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
        public string taxpayersName { get; set; }
        public bool IsWaiver { get; set; }
    }
}
