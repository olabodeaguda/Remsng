using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class LcdaProperty
    {
        public Guid id { get; set; }
        public string propertyKey { get; set; }
        public string propertyValue { get; set; }
        public Guid lcdaId { get; set; }
        public string propertyStatus { get; set; }
    }
}
