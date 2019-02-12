using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class StreetModel
    {
        public StreetModel()
        {
            TaxPayer = new HashSet<TaxPayerModel>();
        }

        public Guid Id { get; set; }
        public Guid WardId { get; set; }
        public string StreetName { get; set; }
        public int? NumberOfHouse { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string StreetStatus { get; set; }
        public string StreetDescription { get; set; }

        public virtual WardModel Ward { get; set; }
        public virtual ICollection<TaxPayerModel> TaxPayer { get; set; }
    }
}
