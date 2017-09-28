using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
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

        public async Task<Role> GetUserRoleByUsernameByDomainId(string username, Guid domainId)
        {
            return await db.Roles.FromSql("sp_getUserRoleByUsername @p0", username).FirstOrDefaultAsync();
        }

        public async Task<bool> Add(Role role)
        {
            db.Roles.Add(role);
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<Role> GetByName(string rolename)
        {
            return await db.Roles.FirstOrDefaultAsync(x => x.roleName == rolename);
        }

        public async Task<Role> GetById(Guid id)
        {
            return await db.Roles.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<bool> Update(Role role)
        {
            var r = await db.Roles.FindAsync(new object[] { role.id });

            if (r == null)
            {
                throw new NotFoundException($"{role.roleName} was not found");
            }

            r.roleName = role.roleName;
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateStatus(Role role)
        {
            var r = await db.Roles.FindAsync(new object[] { role.id });

            if (r == null)
            {
                throw new NotFoundException($"{role.roleName} was not found");
            }

            r.roleStatus = role.roleStatus;
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

    }
}
