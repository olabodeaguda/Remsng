using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public partial class CompanyModel
    {
        public CompanyModel()
        {
            TaxPayer = new HashSet<TaxPayerModel>();
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

        public string sectorName { get; set; }
        public string categoryName { get; set; }

        public virtual ICollection<TaxPayerModel> TaxPayer { get; set; }
    }
}
