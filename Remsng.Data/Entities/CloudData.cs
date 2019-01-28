using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_cloudData")]
    public partial class CloudData
    {
        public Guid Id { get; set; }
        public Guid DomainId { get; set; }
        public string DataTitle { get; set; }
        public string SyncStatus { get; set; }
        public string JsonContent { get; set; }
        public string BillingNumber { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
