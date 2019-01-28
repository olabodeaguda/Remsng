using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_error")]
    public partial class Error
    {
        public Guid Id { get; set; }
        public string ErrorType { get; set; }
        public string Errorvalue { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid OwnerId { get; set; }
    }
}
