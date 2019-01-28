using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_LcdaProperty")]
    public partial class LcdaProperty
    {
        public Guid Id { get; set; }
        public string PropertyKey { get; set; }
        public string PropertyValue { get; set; }
        public Guid LcdaId { get; set; }
        public string PropertyStatus { get; set; }
    }
}
