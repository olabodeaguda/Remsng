using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class TaxpayerExtension : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid streetId { get; set; }
        public Guid addressId { get; set; }
        public string taxpayerStatus { get; set; }
        public string companyName { get; set; }
        public int totalSize { get; set; }
    }
}
