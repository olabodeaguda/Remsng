using System;

namespace RemsNG.Common.Models
{
    public class TaxPayerModel
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? StreetId { get; set; }
        public Guid? AddressId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string TaxpayerStatus { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string companyName { get; set; }
        public string WardName { get; set; }

        public virtual CompanyModel Company { get; set; }
        public virtual StreetModel Street { get; set; }

        public string StreetNumber { get; set; }
        public string StreetName { get; set; }

        public Guid WardId { get; set; }
        public int ItemCount { get; set; }
    }

    public enum TaxPayerEnum
    {
        ACTIVE, DELETED
    }
}
