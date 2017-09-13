using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class RoleDao : AbstractDao
    {
        public RoleDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Role> GetUserRoleByUsername(string username)
        {
            return await db.Roles.FromSql("sp_getUserRoleByUsername @p0", username).FirstOrDefaultAsync();
        }


    }
}
