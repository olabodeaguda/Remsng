using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class ContactDetailModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string ContactValue { get; set; }
        public string ContactType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
