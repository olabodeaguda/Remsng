using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class Ward
    {
        public Guid id { get; set; }
        public string wardName { get; set; }
        public Guid lcdaId { get; set; }
        public string wardStatus { get; set; }
        public string createdBy { get; set; }
        public DateTime? lastModifiedDate { get; set; }
        public DateTime dateCreated { get; set; }
        public string lastmodifiedBy { get; set; }
        [NotMapped]
        public string lcdaName { get; set; }
    }
}
