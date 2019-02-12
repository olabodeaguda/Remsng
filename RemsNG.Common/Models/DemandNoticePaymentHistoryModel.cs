using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public partial class DemandNoticePaymentHistoryModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string BillingNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Charges { get; set; }
        public string PaymentMode { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid BankId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string PaymentStatus { get; set; }
        public bool SyncStatus { get; set; }
        public bool IsWaiver { get; set; }
    }
}
