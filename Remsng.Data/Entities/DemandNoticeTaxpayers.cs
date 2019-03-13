using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_demandNoticeTaxpayers")]
    public class DemandNoticeTaxpayer
    {
        public Guid Id { get; set; }
        public Guid DnId { get; set; }
        public Guid TaxpayerId { get; set; }
        public string TaxpayersName { get; set; }
        public long BillingNumber { get; set; }
        public string AddressName { get; set; }
        public string WardName { get; set; }
        public string LcdaName { get; set; }
        public int BillingYr { get; set; }
        public string DomainName { get; set; }
        public string LcdaAddress { get; set; }
        public string LcdaState { get; set; }
        public string LcdaLogoFileName { get; set; }
        public string CouncilTreasurerSigFilen { get; set; }
        public string RevCoodinatorSigFilen { get; set; }
        public string CouncilTreasurerMobile { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string DemandNoticeStatus { get; set; }
        public bool IsUnbilled { get; set; } = false;
        public int Period { get; set; } = 1;

        public bool IsRunArrears { get; set; }
        public bool IsRunPenalty { get; set; }

        [ForeignKey("DnId")]
        public DemandNotice DemandNotice { get; set; }

        public virtual ICollection<DemandNoticeItem> DemandNoticeItem { get; set; }
    }
}
