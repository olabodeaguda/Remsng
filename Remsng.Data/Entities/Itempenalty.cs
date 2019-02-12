using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_itempenalty")]
    public partial class ItemPenalty
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

        public virtual Item Item { get; set; }
    }
}
