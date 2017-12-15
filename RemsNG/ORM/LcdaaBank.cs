using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class LcdaBank
    {
        public Guid id { get; set; }
        public Guid bankId { get; set; }
        public Guid lcdaId { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
    }
}
