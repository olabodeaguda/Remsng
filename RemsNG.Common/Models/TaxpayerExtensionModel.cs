using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class TaxpayerExtensionModel : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid streetId { get; set; }
        public Guid addressId { get; set; }
        public string taxpayerStatus { get; set; }
        public string companyName { get; set; }
        public string streetNumber { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        public int totalSize { get; set; }
    }
}
