using System;

namespace RemsNG.Common.Models
{
    public class StateModel
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }

        public virtual CountryModel Country { get; set; }
    }
}
