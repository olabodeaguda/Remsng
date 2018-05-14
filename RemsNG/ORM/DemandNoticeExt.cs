using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticeExt
    {
        public Guid id { get; set; }
        public string query { get; set; }
        public string batchNo { get; set; }
        public string demandNoticeStatus { get; set; }
        public int billingYear { get; set; }
        public Guid lcdaId { get; set; }
        public Nullable<int> totalSize { get; set; }
        public DemandNoticeRequest demandNoticeRequest { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
