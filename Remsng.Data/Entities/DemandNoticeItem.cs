using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_demandNoticeItem")]
    public partial class DemandNoticeItem
    {
        public Guid Id { get; set; }
        public string BillingNo { get; set; }
        public Guid dn_taxpayersDetailsId { get; set; }
        public Guid DemandNoticeId { get; set; }
        public Guid TaxpayerId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string ItemStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("dn_taxpayersDetailsId")]
        public DemandNoticeTaxpayer DemandNoticeTaxpayer { get; set; }
        public virtual Item Item { get; set; }

        [ForeignKey("TaxpayerId")]
        public TaxPayer TaxPayer { get; set; }
    }
}
