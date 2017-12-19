using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Item : AbstractModel
    {
        public Guid id { get; set; }
        public string itemDescription { get; set; }

        [DefaultValue("ACTIVE")]
        public string itemStatus { get; set; }
        public Guid lcdaId { get; set; }
        public string itemCode { get; set; }
    }
}
