using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DbResponse
    {
        public Guid id { get; set; }
        public string msg { get; set; }
        public bool success { get; set; }
    }
}
