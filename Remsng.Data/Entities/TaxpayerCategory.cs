using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_taxpayerCategory")]
    public partial class TaxpayerCategory
    {
        public Guid Id { get; set; }
        public string TaxpayerCategoryName { get; set; }
        public Guid LcdaId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
