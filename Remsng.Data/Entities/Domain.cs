using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_domain")]
    public partial class Domain
    {
        public Guid Id { get; set; }
        public string DomainName { get; set; }
        public string DomainCode { get; set; }
        public DateTime Datecreated { get; set; }
        public Guid? AddressId { get; set; }
        public string DomainStatus { get; set; }
        public string DomainType { get; set; }
        public Guid? StateId { get; set; }
    }
}
