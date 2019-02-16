using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_state")]
    public class State
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }

        public virtual Country Country { get; set; }
    }
}
