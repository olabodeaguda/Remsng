using System;

namespace RemsNG.Common.Models
{
    public class TaxpayerExtensionModel2 : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid streetId { get; set; }
        public Guid addressId { get; set; }
        public string taxpayerStatus { get; set; }
        public string companyName { get; set; }
        //public string streetNumber { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        public string CompanyName { get; set; }
        public string StreetName { get; set; }
        public string AddressNumber { get; set; }//streetNumber
        public string WardName { get; set; }
    }
}
