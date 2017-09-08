using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Country
    {
        public Guid id { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
    }
}
