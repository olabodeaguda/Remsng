using System;

namespace RemsNG.Models
{
    public class BankCategory
    {
        public string CatgoryName { get; set; }
        public Guid BankId { get; set; }
    }
    public class BankCategories
    {
        public BankCategory[] Categories { get; set; }
    }
}
