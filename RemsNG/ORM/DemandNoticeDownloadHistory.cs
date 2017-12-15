using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticeDownloadHistory:AbstractModel
    {
        public Guid id { get; set; }
        public string billingNumber { get; set; }
        public decimal grandTotal { get; set; }
        public decimal charges { get; set; }
    }
}
