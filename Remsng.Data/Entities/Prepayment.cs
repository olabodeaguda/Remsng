using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Remsng.Data.Entities
{
    [Table("tbl_prepayment")]
    public class Prepayment
    {
        public long id { get; set; }
        public Guid taxpayerId { get; set; }
        public decimal amount { get; set; }
        public DateTime datecreated { get; set; }
        public string prepaymentStatus { get; set; }
    }
}
