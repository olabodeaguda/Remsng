using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class DNAmountDueModel
    {
        public Guid id { get; set; }
        public decimal itemAmount { get; set; }
        public decimal amountPaid { get; set; }
        public string itemDescription { get; set; }
        public string category { get; set; }
        public string itemStatus { get; set; }
        public Guid itemId { get; set; }

        [NotMapped]
        public decimal amountInitialPaid { get; set; }

        public long billingNo { get; set; }
    }
}
