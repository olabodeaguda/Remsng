using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Taxpayer : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid streetId { get; set; }
        public Guid addressId { get; set; }
        public string taxpayerStatus { get; set; }
    }
}
