using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_bank_lcda")]
    public partial class BankLcda
    {
        public Guid Id { get; set; }
        public Guid? BankId { get; set; }
        public Guid? LcdaId { get; set; }
        public string BankAccount { get; set; }
    }
}
