using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    public class State
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }

        public virtual Country Country { get; set; }
    }
}
