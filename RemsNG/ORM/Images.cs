using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Images : AbstractModel
    {
        public Guid id { get; set; }
        public string imgFilename { get; set; }
        public Guid ownerId { get; set; }
        public string imgType { get; set; }
    }
}
