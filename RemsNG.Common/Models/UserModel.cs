﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public DateTime? LockedOutEndDateUtc { get; set; }
        public bool? Lockedoutenabled { get; set; }
        public string Username { get; set; }
        public string UserStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Lastname { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Gender { get; set; }
    }
}
