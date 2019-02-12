using System;
using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public partial class WardModel
    {
        public WardModel()
        {
            Street = new HashSet<StreetModel>();
        }

        public Guid Id { get; set; }
        public string WardName { get; set; }
        public Guid LcdaId { get; set; }
        public string WardStatus { get; set; }
        public string CreatedBy { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual LcdaModel Lcda { get; set; }
        public virtual ICollection<StreetModel> Street { get; set; }
    }
}
