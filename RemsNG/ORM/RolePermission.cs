using System;

namespace RemsNG.ORM
{
    public class RolePermission
    {
        public Guid roleId { get; set; }
        public Guid permissionId { get; set; }
    }
}
