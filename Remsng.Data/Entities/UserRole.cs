﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_userRole")]
    public class UserRole
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
