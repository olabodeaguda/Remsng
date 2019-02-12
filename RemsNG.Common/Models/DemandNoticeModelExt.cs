using System;

namespace RemsNG.Common.Models
{
    public class DemandNoticeModelExt
    {
        public Guid id { get; set; }
        public string query { get; set; }
        public string batchNo { get; set; }
        public string demandNoticeStatus { get; set; }
        public int billingYear { get; set; }
        public Guid lcdaId { get; set; }
        public Nullable<int> totalSize { get; set; }
        public DemandNoticeRequestModel demandNoticeRequestModel { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
