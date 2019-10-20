using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Models
{
    public class AmountDueModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public string Id { get; set; }
    }

    public enum Category
    {
        Item, Arrears, Penalty, Prepayment, Paid
    }

}
