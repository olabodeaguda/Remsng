using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_ward")]
    public partial class Ward
    {
        public Ward()
        {
            Street = new HashSet<Street>();
        }

        public Guid Id { get; set; }
        public string WardName { get; set; }
        public Guid LcdaId { get; set; }
        public string WardStatus { get; set; }
        public string CreatedBy { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual Lcda Lcda { get; set; }
        public virtual ICollection<Street> Street { get; set; }
    }
}
