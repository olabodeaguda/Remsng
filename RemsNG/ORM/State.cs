using System;
using System.ComponentModel.DataAnnotations;

namespace RemsNG.ORM
{
    public class State
    {
        [Key]
        public Guid id { get; set; }
        public Guid countryId { get; set; }
        public string stateCode { get; set; }
        public string stateName { get; set; }
    }
}