using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_demandNoticePenalty")]
    public partial class DemandNoticePenalty
    {
        public Guid Id { get; set; }
        public string BillingNo { get; set; }
        public Guid TaxpayerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public Guid ItemId { get; set; }
        public int OriginatedYear { get; set; }
        public int BillingYear { get; set; }
        public string ItemPenaltyStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
