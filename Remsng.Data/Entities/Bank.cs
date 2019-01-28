using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_bank")]
    public partial class Bank
    {
        public Guid Id { get; set; }
        public string BankName { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
