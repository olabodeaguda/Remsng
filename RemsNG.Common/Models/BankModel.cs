using System;

namespace RemsNG.Common.Models
{
    public class BankModel
    {
        public Guid Id { get; set; }
        public string BankName { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
