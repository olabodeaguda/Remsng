using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNotice:AbstractModel
    {
        public Guid id { get; set; }
        public string query { get; set; }
        public string batchNo { get; set; }
        public string demandNoticeStatus { get; set; }
        public int billingYear { get; set; }
        public Guid lcdaId { get; set; }
        public Nullable<int> totalSize { get; set; }
        public Guid? wardId { get; set; }
        public Guid? streetId { get; set; }

    }
}
