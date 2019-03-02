using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_street")]
    public partial class Street
    {
        public Street()
        {
            TaxPayer = new HashSet<TaxPayer>();
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

        public virtual Ward Ward { get; set; }
        public virtual ICollection<TaxPayer> TaxPayer { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
