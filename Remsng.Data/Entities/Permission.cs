using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_permission")]
    public partial class Permission
    {
        public Guid Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }


        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    }
}
