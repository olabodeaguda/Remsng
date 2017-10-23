using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Company: AbstractModel
    {
        public Guid id { get; set; }
        public Guid lcdaId { get; set; }
        public string companyName { get; set; }
        public Nullable<Guid> sectorId { get; set; }
        public Nullable<Guid> addressId { get; set; }
        public Nullable<Guid> categoryId { get; set; }
        public Nullable<Guid> streetId { get; set; }
        public string companyStatus { get; set; }
    }
}
