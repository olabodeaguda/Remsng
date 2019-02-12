using System;

namespace RemsNG.Common.Models
{
    public class DemandNoticeItemPenaltyModelExt : AbstractModel
    {
        public Guid id { get; set; }
        public string billingNo { get; set; }
        public Guid taxpayerId { get; set; }
        public decimal totalAmount { get; set; }
        public decimal amountPaid { get; set; }
        public Guid itemId { get; set; }
        public int originatedYear { get; set; }// billing year arrears
        public int billingYear { get; set; }
        public string itemPenaltyStatus { get; set; }
        public string category { get; set; }
        public string wardName { get; set; }
    }
}
