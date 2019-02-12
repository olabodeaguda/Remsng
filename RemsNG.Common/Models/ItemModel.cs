using System;
using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public class ItemModel
    {
        public ItemModel()
        {
            CompanyItem = new HashSet<CompanyItemModel>();
            DemandNoticeItem = new HashSet<DemandNoticeItemModel>();
            Itempenalty = new HashSet<ItemPenaltyModel>();
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

        public virtual LcdaModel Lcda { get; set; }
        public virtual ICollection<CompanyItemModel> CompanyItem { get; set; }
        public virtual ICollection<DemandNoticeItemModel> DemandNoticeItem { get; set; }
        public virtual ICollection<ItemPenaltyModel> Itempenalty { get; set; }
    }
}
