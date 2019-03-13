using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class DemandNoticePenaltyModel
    {
        public Guid Id { get; set; }
        public long BillingNo { get; set; }
        public Guid TaxpayerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public int OriginatedYear { get; set; }
        public int BillingYear { get; set; }
        public string ItemPenaltyStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string category { get; set; }
        public string wardName { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
