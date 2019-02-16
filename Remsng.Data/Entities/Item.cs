using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_item")]
    public partial class Item
    {
        public Item()
        {
            CompanyItem = new HashSet<CompanyItem>();
            DemandNoticeItem = new HashSet<DemandNoticeItem>();
            Itempenalty = new HashSet<ItemPenalty>();
        }

        public Guid Id { get; set; }
        public string ItemDescription { get; set; }
        public string ItemStatus { get; set; }
        public Guid LcdaId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ItemCode { get; set; }

        public virtual Lcda Lcda { get; set; }
        public virtual ICollection<CompanyItem> CompanyItem { get; set; }
        public virtual ICollection<DemandNoticeItem> DemandNoticeItem { get; set; }
        public virtual ICollection<ItemPenalty> Itempenalty { get; set; }
    }
}
