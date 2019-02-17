using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class AddressModel
    {
        public Guid Id { get; set; }
        public string Addressnumber { get; set; }
        public Guid StreetId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid OwnerId { get; set; }
        public Guid Lcdaid { get; set; }
        public string StreetName { get; set; }
    }
}
