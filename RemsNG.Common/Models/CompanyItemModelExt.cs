﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class CompanyItemModelExt : AbstractModel
    {
        public Guid id { get; set; }
        public Guid taxpayerId { get; set; }
        public Guid itemId { get; set; }
        public decimal amount { get; set; }
        public int billingYear { get; set; }
        public string companyStatus { get; set; }
        public string itemName { get; set; }
        public int totalSize { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}