using System;

namespace RemsNG.Common.Models
{
    public class ItemPenaltyModel
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public bool IsPercentage { get; set; }
        public string PenaltyStatus { get; set; }
        public decimal Amount { get; set; }
        public string Duration { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual ItemModel Item { get; set; }
    }
}
