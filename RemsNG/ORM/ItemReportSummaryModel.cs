using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class ItemReportSummaryModel
    {
        public Guid id { get; set; }
        public decimal itemAmount { get; set; }
        public decimal amountPaid { get; set; }
        public string billingNo { get; set; }
        public string category { get; set; }
        public Guid wardId { get; set; }
        public string wardName { get; set; }
        public string itemDescription { get; set; }
        public string itemCode { get; set; }
        public string taxpayersName { get; set; }
        public string addressName { get; set; }
        public DateTime? lastModifiedDate { get; set; }
    }
}
