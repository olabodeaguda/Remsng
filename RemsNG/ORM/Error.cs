using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Error
    {
        public Guid id { get; set; }
        public Guid ownerId { get; set; }
        public string errorType { get; set; }
        public string errorvalue { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
