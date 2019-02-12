using System;
using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public class DemandNoticeReportModel : DemandNoticeTaxpayersModel
    {
        public List<DnReportItemModel> items { get; set; } = new List<DnReportItemModel>();
        public string amountInWords { get; set; }
        public List<BankLcdaModel> banks { get; set; } = new List<BankLcdaModel>();
        public decimal penalty { get; set; }
        public decimal arrears { get; set; }
        public Guid lcdaId { get; set; }
        public decimal charges { get; set; }
        public decimal amountPaid { get; set; }
        public decimal amountDue { get; set; }
    }
}
