using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class TaxpayerCategory : AbstractModel
    {
        public Guid id { get; set; }
        public string taxpayerCategoryName { get; set; }
        public Guid lcdaId { get; set; }
    }
}
