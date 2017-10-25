using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class CompanyItemExt
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid itemId { get; set; }
        public decimal amount { get; set; }
        public int billingYear { get; set; }
        public string companyStatus { get; set; }
        public string companyName { get; set; }
        public string itemName { get; set; }
    }
}
