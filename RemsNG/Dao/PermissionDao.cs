using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class PermissionDao : AbstractDao
    {
        public PermissionDao(RemsDbContext _db) : base(_db)
        {
        }

        public List<Permission> byRoleId(Guid roleId)
        {
            return db.Permissions.FromSql("sp_getRolePermission @p0", new object[] { roleId }).ToList();
        }

    }
}
