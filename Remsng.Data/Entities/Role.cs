using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_role")]
    public partial class Role
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public Guid DomainId { get; set; }
        public string RoleStatus { get; set; }

        [ForeignKey("DomainId")]
        public virtual Lcda Domain { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
