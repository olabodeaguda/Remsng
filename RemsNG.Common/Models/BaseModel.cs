using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class BaseModel
    {
        public string createdBy { get; set; }
        public DateTimeOffset dateCreated { get; set; } = DateTimeOffset.Now;
        public string lastmodifiedby { get; set; }
        public DateTime? lastModifiedDate { get; set; }
    }
}
