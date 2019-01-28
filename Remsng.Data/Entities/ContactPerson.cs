using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_contactPerson")]
    public partial class ContactPerson
    {
        public Guid Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Guid? TaxPayerId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
