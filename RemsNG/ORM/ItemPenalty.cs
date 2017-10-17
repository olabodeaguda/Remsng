using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class ItemPenalty:AbstractModel
    {
        public Guid id { get; set; }
        public Guid itemId { get; set; }
        public bool isPercentage { get; set; }
        [DefaultValue("ACTIVE")]
        public string penaltyStatus { get; set; }
        public decimal amount { get; set; }
        public string duration { get; set; }
    }
}
