using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_users")]
    public partial class User
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

        public ICollection<UserDomain> UserDomains { get; set; } = new HashSet<UserDomain>();
        public ICollection<UserLcda> UserLcdas { get; set; } = new HashSet<UserLcda>();
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
