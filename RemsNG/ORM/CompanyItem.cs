using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class CompanyItem : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid itemId { get; set; }
        public decimal amount { get; set; }
        public int billingYear { get; set; }
        public string companyStatus { get; set; }
    }
}
