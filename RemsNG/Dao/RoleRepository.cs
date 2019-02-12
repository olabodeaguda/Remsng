using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class RoleRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public RoleRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Role Dao");
        }

        public async Task<RoleExtensionModel> GetUserDomainRoleByUsername(string username, Guid domainId)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRoleByUsername @p0, @p1", new object[] { username, domainId }).FirstOrDefaultAsync();
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                List<RoleExtensionModel> AllRole = db.RoleExtensions.FromSql("sp_getRoles @p0, @p1", new object[] { pageModel.PageSize, pageModel.PageNum }).ToList();

                var totalCount = db.Roles.Count();
                return new
                {
                    data = AllRole,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<object> Paginated(PageModel pageModel, Guid domainId)
        {
            List<RoleExtensionModel> AllRole = db.RoleExtensions.FromSql("sp_getRolesByDomainIdPaginated @p0, @p1, @p2", new object[] { pageModel.PageSize, pageModel.PageNum, domainId }).ToList();

            var totalCount = await db.Roles.Where(x => x.DomainId == domainId).CountAsync();
            return new
            {
                data = AllRole,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<bool> Add(Role role)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createRole @p0,@p1, @p2",
                new object[] { role.Id, role.RoleName, role.DomainId }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return true;
            }
            logger.LogError(dbResponse.msg, new object[] { role.RoleName, role.Id });
            return false;
        }

        public async Task<UserRole> GetRoleAsync(UserRole userRole)
        {
            return await db.UserRoles.FirstOrDefaultAsync(x => x.RoleId == userRole.RoleId && x.UserId == userRole.UserId);
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

        public async Task<RoleExtensionModel> GetByName(string rolename)
        {
            return await db.RoleExtensions.FromSql("sp_roleByRoleName @p0", new object[] { rolename }).FirstOrDefaultAsync();
        }

        public async Task<RoleExtensionModel> GetById(Guid id)
        {
            return await db.RoleExtensions.FromSql("sp_roleById @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(Role role)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateRole @p0, @p1, @p2",
                new object[] { role.RoleName, role.Id, role.DomainId }).FirstOrDefaultAsync();
            if (dbResponse.success)
            {
                return true;
            }
            logger.LogError(dbResponse.msg, new object[] { role.RoleName, role.Id });
            return false;
        }

        public async Task<bool> UpdateStatus(Role role)
        {
            var r = await GetById(role.Id);

            if (r == null)
            {
                throw new NotFoundException($"{role.RoleName} was not found");
            }

            r.roleName = role.RoleName;
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateRoleStatus",
                new object[] { role.RoleStatus, role.Id }).FirstOrDefaultAsync();
            if (dbResponse.success)
            {
                return true;
            }
            logger.LogError(dbResponse.msg, new object[] { role.RoleName, role.Id });
            return false;
        }

        public async Task<List<RoleExtensionModel>> ByDomainId(Guid domainId)
        {
            return await db.RoleExtensions.FromSql("sp_roleByDomainId @p0", new object[] { domainId }).ToListAsync();
        }

        public async Task<List<RoleExtensionModel>> AllRoleByUserId(Guid id)
        {
            return await db.RoleExtensions.FromSql("sp_getDomainRolesByUserId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<List<RoleExtensionModel>> AllRoleByUsername(string username)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRoleByUsername @p0", new object[] { username }).ToListAsync();
        }

        public async Task<List<RoleExtensionModel>> AllDomainRolesByUsername(string username)
        {
            return await db.RoleExtensions.FromSql("sp_getDomainRolesByUsername @p0", new object[] { username }).ToListAsync();
        }

        public async Task<RoleExtensionModel> UserDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            return await db.RoleExtensions.FromSql("sp_getUserDomainRolesByDomainId @p0, @p1", new object[] { userid, domainid }).FirstOrDefaultAsync();
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
            return await db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
        }

        public async Task<bool> Remove(UserRole userRole)
        {
            var r = await db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userRole.UserId && x.RoleId == userRole.RoleId);

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
