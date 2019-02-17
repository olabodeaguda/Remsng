using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_companyItem")]
    public partial class CompanyItem
    {
        public Guid Id { get; set; }
        public Guid TaxpayerId { get; set; }
        public Guid ItemId { get; set; }
        public decimal Amount { get; set; }
        public int BillingYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CompanyStatus { get; set; }

        public virtual Item Item { get; set; }
        [ForeignKey("TaxpayerId")]
        public TaxPayer TaxPayer { get; set; }
    }
}
