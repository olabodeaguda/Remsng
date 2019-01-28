using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_country")]
    public partial class Country
    {
        public Country()
        {
            State = new HashSet<State>();
        }

        public Guid Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }

        public virtual ICollection<State> State { get; set; }
    }
}
