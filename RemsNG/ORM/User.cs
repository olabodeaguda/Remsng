using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class User: AbstractModel
    {
        [Key]
        public Guid id { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string securityStamp { get; set; }
        public DateTime? lockedOutEndDateUTC { get; set; }
        public bool lockedoutenabled { get; set; }
        public string username { get; set; }
        public string userStatus { get; set; }
        public string surname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}
