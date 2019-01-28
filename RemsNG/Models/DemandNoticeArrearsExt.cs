using RemsNG.ORM;
using System;

namespace RemsNG.Models
{
    public class DemandNoticeArrearsExt : AbstractModel
    {
        public Guid id { get; set; }
        public Guid taxpayerId { get; set; }
        public string billingNo { get; set; }
        public decimal totalAmount { get; set; }
        public decimal amountPaid { get; set; }
        public Guid itemId { get; set; }
        public int originatedYear { get; set; }
        public string arrearsStatus { get; set; }
        public string category { get; set; }
        public string wardName { get; set; }
    }
}
