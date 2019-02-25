using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_company")]
    public partial class Company
    {
        public Company()
        {
            TaxPayer = new HashSet<TaxPayer>();
        }

        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public Guid? StreetId { get; set; }
        public Guid? SectorId { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? CategoryId { get; set; }
        public string CompanyStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid LcdaId { get; set; }

        public virtual ICollection<TaxPayer> TaxPayer { get; set; }
        public TaxpayerCategory TaxPayerCatgeory { get; set; }
    }
}
