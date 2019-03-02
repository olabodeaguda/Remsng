using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_demandnotice")]
    public class DemandNotice
    {
        public DemandNotice()
        {
        }

        public Guid Id { get; set; }
        public string Query { get; set; }
        public string PlainTextQuery { get; set; }
        public string BatchNo { get; set; }
        public string DemandNoticeStatus { get; set; }
        public int BillingYear { get; set; }
        public Guid LcdaId { get; set; } = Guid.Empty;
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? WardId { get; set; }
        public Guid? StreetId { get; set; }
        public bool IsUnbilled { get; set; } = false;

        [ForeignKey("StreetId")]
        public Street Street { get; set; }

        public virtual ICollection<DemandNoticeTaxpayer> DemandNoticeTaxpayers { get; set; }

        [ForeignKey("WardId")]
        public Ward Ward { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
