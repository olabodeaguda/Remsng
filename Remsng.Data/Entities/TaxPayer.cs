using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_taxPayer")]
    public partial class TaxPayer
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? StreetId { get; set; }
        public Guid? AddressId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string TaxpayerStatus { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public virtual Company Company { get; set; }
        public virtual Street Street { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
