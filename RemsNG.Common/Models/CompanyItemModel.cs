﻿using System;

namespace RemsNG.Common.Models
{
    public partial class CompanyItemModel
    {
        public Guid Id { get; set; }
        public Guid TaxpayerId { get; set; }
        public Guid ItemId { get; set; }
        public decimal Amount { get; set; }
        public int BillingYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CompanyStatus { get; set; }

        public virtual ItemModel Item { get; set; }

        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ItemName { get; set; }
    }

    public enum CompanyStatus
    {
        ACTIVE,NOT_ACTIVE
    }
}
