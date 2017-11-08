using System;
using System.ComponentModel.DataAnnotations;

namespace RemsNG.ORM
{
    public class Taxpayer : AbstractModel
    {
        public Guid id { get; set; }
        public Guid companyId { get; set; }
        public Guid streetId { get; set; }
        public Nullable<Guid> addressId { get; set; }
        public string taxpayerStatus { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string streetNumber { get; set; }
    }
}
