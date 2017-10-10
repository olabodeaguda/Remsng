using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.Models;
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

        public async Task<RoleExtension> GetUserDomainRoleByUsername(string username, Guid domainId)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRoleByUsername @p0, @p1", new object[] { username, domainId }).FirstOrDefaultAsync();
        }

        public async Task<object> Paginated(Models.PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                List<RoleExtension> AllRole = db.RoleExtensions.FromSql("sp_getRoles @p0, @p1", new object[] { pageModel.PageSize, pageModel.PageNum }).ToList();

                var totalCount = db.Roles.Count();
                return new
                {
                    data = AllRole,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<bool> Add(Role role)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createRole @p0,@p1, @p2",
                new object[] { role.id, role.roleName, role.domainId }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return true;
            }
            return false;
        }

        public async Task<UserRole> GetRoleAsync(UserRole userRole)
        {
            return await db.UserRoles.FirstOrDefaultAsync(x => x.roleid == userRole.roleid && x.userid == userRole.userid);
        }

        public async Task<bool> Add(RolePermission role)
        {
            db.RolePermissions.Add(role);
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

        public async Task<RoleExtension> GetById(Guid id)
        {
            Role role = await db.Roles.Include("lgda").FirstOrDefaultAsync(x => x.id == id);

            if (role != null)
            {
                RoleExtension roleExtension = new RoleExtension()
                {
                    domainId = role.domainId,
                    id = role.id,
                    roleName = role.roleName,
                    roleStatus = role.roleStatus
                };
                roleExtension.domainName = role.lgda.lcdaName;

                return roleExtension;
            }

            return null;
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

        public async Task<List<Role>> ByDomainId(Guid domainId)
        {
            return await db.Roles.Where(x => x.domainId == domainId).ToListAsync();
        }

        public async Task<List<RoleExtension>> AllRoleByUserId(Guid id)
        {
            return await db.RoleExtensions.FromSql("sp_getDomainRolesByUserId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<List<RoleExtension>> AllRoleByUsername(string username)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRoleByUsername @p0", new object[] { username }).ToListAsync();
        }

        public async Task<List<RoleExtension>> AllDomainRolesByUsername(string username)
        {
            return await db.RoleExtensions.FromSql("sp_getDomainRolesByUsername @p0", new object[] { username }).ToListAsync();
        }

        public async Task<List<RoleExtension>> AllDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRolesByDomainId @p0, @p1", new object[] { userid, domainid }).ToListAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(UserRole userRole)
        {
            db.UserRoles.Add(userRole);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<UserRole> GetUserRoleAsync(Guid userId, Guid roleId)
        {
            return await db.UserRoles.FirstOrDefaultAsync(x => x.userid == userId && x.roleid == roleId);
        }

        public async Task<bool> Remove(UserRole userRole)
        {
            var r = await db.UserRoles.FirstOrDefaultAsync(x => x.userid == userRole.userid && x.roleid == userRole.roleid);

            db.UserRoles.Remove(r);

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
