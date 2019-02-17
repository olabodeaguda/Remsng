using System;

namespace RemsNG.Common.Models
{
    public class ChartReportModel
    {
        public Guid id { get; set; }
        public string wardName { get; set; }
        public decimal itemAmount { get; set; }
        public decimal amountPaid { get; set; }
        public int BillingYear { get; set; }
    }
}
