using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DemandNoticeReportModel: DemandNoticeTaxpayersDetail
    {
        public List<DnReportItem> items { get; set; } = new List<DnReportItem>();
        public string amountInWords { get; set; }
        public List<LcdaBank> banks { get; set; } = new List<LcdaBank>();
        public decimal penalty { get; set; }
        public decimal arrears { get; set; }
        public Guid lcdaId { get; set; }
    }
}
