using System;

namespace RemsNG.Common.Models
{
    public class BankLcdaModel
    {
        public Guid id { get; set; }
        public Guid bankId { get; set; }
        public Guid lcdaId { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }

    }
}
