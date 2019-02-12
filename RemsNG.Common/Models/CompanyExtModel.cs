using System;

namespace RemsNG.Common.Models
{
    public class CompanyExtModel : AbstractModel
    {
        public Guid id { get; set; }
        public Guid lcdaId { get; set; }
        public string companyName { get; set; }
        public Nullable<Guid> sectorId { get; set; }
        public Nullable<Guid> addressId { get; set; }
        public Nullable<Guid> categoryId { get; set; }
        public Nullable<Guid> streetId { get; set; }
        public string companyStatus { get; set; }

        public string sectorName { get; set; }
        public string categoryName { get; set; }

        //[NotMapped]
        public Nullable<int> totalSize { get; set; }
    }
}
