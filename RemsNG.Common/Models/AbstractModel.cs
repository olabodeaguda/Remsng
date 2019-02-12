using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Models
{
    public class AbstractModel
    {
        public string createdBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public string lastmodifiedby { get; set; }
        public DateTime? lastModifiedDate { get; set; }
    }
}
