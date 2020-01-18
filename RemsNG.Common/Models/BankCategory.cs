using System;

namespace RemsNG.Common.Models
{
    public class BankCategory
    {
        public string CatgoryAccountNumbers { get; set; }
        public string DefaultAccountNumber { get; set; }
        public int RecieptType { get; set; }
    }
    public class BankCategories
    {
        public BankCategory[] Categories { get; set; }
    }
}
