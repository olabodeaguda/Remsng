using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_DemandNoticeDownloadHistory")]
    public partial class DemandNoticeDownloadHistory
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
