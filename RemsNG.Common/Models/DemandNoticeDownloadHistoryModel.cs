using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public partial class DemandNoticeDownloadHistoryModel
    {
        public Guid Id { get; set; }
        public long BillingNumber { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Charges { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
