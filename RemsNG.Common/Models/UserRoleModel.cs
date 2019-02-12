using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class UserRoleModel
    {
        public Guid RoleId { get; set; }
        public RoleModel Role { get; set; }
        public Guid UserId { get; set; }
        public UserModel User { get; set; }
    }
}
