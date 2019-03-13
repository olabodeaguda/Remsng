using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Infrastructure.Models
{
    public class RequestAmount
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Guid ItemId { get; set; }
    }
}
