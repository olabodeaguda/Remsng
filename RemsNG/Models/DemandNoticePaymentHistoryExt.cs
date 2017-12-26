using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DemandNoticePaymentHistoryExt: DemandNoticePaymentHistory
    {
        public int totalSize { get; set; }
    }
}
