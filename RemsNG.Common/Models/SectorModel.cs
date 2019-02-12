using System;

namespace RemsNG.Common.Models
{
    public class SectorModel
    {
        public Guid Id { get; set; }
        public string SectorName { get; set; }
        public Guid LcdaId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Prefix { get; set; }

        public virtual LcdaModel Lcda { get; set; }
    }
}
