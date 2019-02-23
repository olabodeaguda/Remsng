using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_sector")]
    public partial class Sector
    {
        public Guid Id { get; set; }
        public string SectorName { get; set; }
        public Guid LcdaId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Prefix { get; set; }

        public virtual Lcda Lcda { get; set; }

    }
}
