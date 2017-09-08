using RemsNG.ORM;
using System;

namespace RemsNG.ORM
{
    public class Taxpayer : AbstractModel
    {
        public Guid id { get; set; }
        public string companyName { get; set; }
        public Guid lcdaId { get; set; }
        public Guid sectorId { get; set; }
        public Guid addressId { get; set; }
        public Guid categoryId { get; set; }
    }
}
