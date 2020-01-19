using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.RequestModel
{
    public class WardStreetVm
    {
        public Guid WardId { get; set; }
        public Guid StreetId { get; set; }
        public Guid[] TaxpayerIds { get; set; }
    }
}
