using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class User : AbstractModel
    {
        [Key]
        public Guid id { get; set; }
        [EmailAddress(ErrorMessage ="Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string securityStamp { get; set; }
        public DateTime? lockedOutEndDateUTC { get; set; }
        public bool lockedoutenabled { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string username { get; set; }
        public string userStatus { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname is required")]
        public string surname { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        public string firstname { get; set; }
        public string lastname { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is required")]
        public string gender { get; set; }
    }
}
