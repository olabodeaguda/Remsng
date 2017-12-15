using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class BatchDemandNoticeModel:AbstractModel
    {
        public Guid id { get; set; }
        public string batchNo { get; set; }
        public string requestStatus { get; set; }
        public Guid? lcdaId { get; set; }
        public string batchFileName { get; set; }
    }
}
