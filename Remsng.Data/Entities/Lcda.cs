using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_lcda")]
    public partial class Lcda
    {
        public Lcda()
        {
            Item = new HashSet<Item>();
            Role = new HashSet<Role>();
            Sector = new HashSet<Sector>();
            Ward = new HashSet<Ward>();
        }

        public Guid Id { get; set; }
        public Guid DomainId { get; set; }
        public string LcdaName { get; set; }
        public string LcdaCode { get; set; }
        public Guid AddressId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LcdaStatus { get; set; }
        public decimal Charges { get; set; }

        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Role> Role { get; set; }
        public virtual ICollection<Sector> Sector { get; set; }
        public virtual ICollection<Ward> Ward { get; set; }

        public Domain Domain { get; set; }

        public ICollection<UserLcda> UserLcdas { get; set; } = new HashSet<UserLcda>();
    }
}
