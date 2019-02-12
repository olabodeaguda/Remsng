using System;
using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public partial class CountryModel
    {
        public CountryModel()
        {
            State = new HashSet<StateModel>();
        }

        public Guid Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }

        public virtual ICollection<StateModel> State { get; set; }
    }
}
