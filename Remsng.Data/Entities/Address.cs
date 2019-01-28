using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_address")]
    public partial class Address
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
    }
}
