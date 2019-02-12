using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class LcdaPropertyModel
    {
        public Guid Id { get; set; }
        public string PropertyKey { get; set; }
        public string PropertyValue { get; set; }
        public Guid LcdaId { get; set; }
        public string PropertyStatus { get; set; }
    }
}
