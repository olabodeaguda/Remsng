using System;

namespace RemsNG.Common.Models
{
    public class RolePermissionModel
    {
        public Guid RoleId { get; set; }
        public RoleModel Role { get; set; }
        public Guid PermissionId { get; set; }
        public PermissionModel Permission { get; set; }
    }
}
