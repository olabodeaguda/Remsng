using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old Password is required")]
        public string oldPwd { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password is required")]
        public string newPwd { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please confirm your Password is required")]
        public string confirmPwd { get; set; }

        public Guid id { get; set; }
    }
}
