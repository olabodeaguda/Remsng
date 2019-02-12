using System;
using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public class LcdaModel
    {
        public LcdaModel()
        {
            Item = new HashSet<ItemModel>();
            Role = new HashSet<RoleModel>();
            Sector = new HashSet<SectorModel>();
            Ward = new HashSet<WardModel>();
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

        public virtual ICollection<ItemModel> Item { get; set; }
        public virtual ICollection<RoleModel> Role { get; set; }
        public virtual ICollection<SectorModel> Sector { get; set; }
        public virtual ICollection<WardModel> Ward { get; set; }
    }
}
