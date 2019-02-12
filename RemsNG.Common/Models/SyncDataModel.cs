using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class SyncDataModel : AbstractModel
    {
        public Guid Id { get; set; }
        public string taxpayerName { get; set; }
        public string bankName { get; set; }
        [NotMapped]
        public Guid domainId { get; set; }
        public Guid ownerId { get; set; }
        public string billingNumber { get; set; }
        public decimal amount { get; set; }
        public decimal charges { get; set; }
        public string paymentMode { get; set; }
        public string referenceNumber { get; set; }
        public Guid bankId { get; set; }
        public string paymentStatus { get; set; }

        public bool syncStatus { get; set; }
        //public string Category { get; set; }
    }
}
