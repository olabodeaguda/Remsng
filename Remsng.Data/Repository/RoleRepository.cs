using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class RoleRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public RoleRepository(DbContext _db,
            ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Role Dao");
        }

        public async Task<RoleModel> GetUserDomainRoleByUsername(string username, Guid domainId)
        {
            var result = await db.Set<UserRole>()
                .Include(x => x.User)
                .Include(x => x.Role)
                .Include(x => x.Role.Domain)
                .FirstOrDefaultAsync(x => x.User.Username == username && x.Role.DomainId == domainId);
            if (result == null)
            {
                return null;
            }

            return new RoleModel
            {
                DomainId = result.Role.DomainId,
                domainName = result.Role.Domain.LcdaName,
                Id = result.Role.Id,
                RoleName = result.Role.RoleName,
                RoleStatus = result.Role.RoleStatus
            };

            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_getUserDomainRoleByUsername @p0, @p1", new object[] { username, domainId }).FirstOrDefaultAsync();
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            var query = db.Set<Role>().Include(x => x.Domain);

            var roles = await query.Select(x => new RoleModel
            {
                domainName = x.Domain.LcdaName,
                Id = x.Id,
                RoleName = x.RoleName,
                RoleStatus = x.RoleStatus
            }).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize)
            .OrderByDescending(x => x.RoleName)
            .ToListAsync();

            int totalCount = await query.CountAsync();
            return new
            {
                data = roles,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<object> Paginated(PageModel pageModel, Guid domainId)
        {
            //List<RoleExtensionModel> AllRole = db.Set<RoleExtensionModel>()
            //    .FromSql("sp_getRolesByDomainIdPaginated @p0, @p1, @p2",
            //    new object[] { pageModel.PageSize, pageModel.PageNum, domainId }).ToList();
            var query = db.Set<Role>().Include(x => x.Domain).Where(x => x.DomainId == domainId);
            var totalCount = await query.CountAsync();
            var roles = await query.Select(x => new RoleModel
            {
                domainName = x.Domain.LcdaName,
                Id = x.Id,
                RoleName = x.RoleName,
                RoleStatus = x.RoleStatus
            }).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize)
            .OrderByDescending(x => x.RoleName)
            .ToListAsync();
            return new
            {
                data = roles,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<bool> Add(RoleModel role)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_createRole @p0,@p1, @p2",
            //    new object[] { role.Id, role.RoleName, role.DomainId }).FirstOrDefaultAsync();

            db.Set<Role>().Add(new Role
            {
                DomainId = role.DomainId,
                Id = role.Id,
                RoleName = role.RoleName
            });
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Add(RolePermissionModel role)
        {
            db.Set<RolePermission>().Add(new RolePermission()
            {
                PermissionId = role.PermissionId,
                RoleId = role.RoleId
            });
            int count = await db.SaveChangesAsync();

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<RoleModel> GetByName(string rolename)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_roleByRoleName @p0", new object[] { rolename }).FirstOrDefaultAsync();
            return await db.Set<Role>().Include(x => x.Domain).Select(d => new RoleModel
            {
                domainName = d.Domain.LcdaName,
                Id = d.Id,
                RoleName = d.RoleName,
                RoleStatus = d.RoleStatus
            }).FirstOrDefaultAsync(x => x.RoleName == rolename);
        }

        public async Task<RoleModel> GetById(Guid id)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_roleById @p0", new object[] { id }).FirstOrDefaultAsync();
            return await db.Set<Role>().Include(x => x.Domain).Select(d => new RoleModel
            {
                domainName = d.Domain.LcdaName,
                Id = d.Id,
                RoleName = d.RoleName,
                RoleStatus = d.RoleStatus
            }).FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<bool> Update(RoleModel role)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_updateRole @p0, @p1, @p2",
            //    new object[] { role.RoleName, role.Id, role.DomainId }).FirstOrDefaultAsync();
            //if (dbResponse.success)
            //{
            //    return true;
            //}
            //logger.LogError(dbResponse.msg, new object[] { role.RoleName, role.Id });
            //return false;

            var entity = await db.Set<Role>().FindAsync(role.Id);
            if (entity == null)
            {
                throw new NotFoundException("Role does not exist");
            }
            entity.RoleName = role.RoleName;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatus(RoleModel role)
        {

            var entity = await db.Set<Role>().FindAsync(role.Id);
            if (entity == null)
            {
                throw new NotFoundException("Role does not exist");
            }
            entity.RoleStatus = role.RoleStatus;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleModel>> ByDomainId(Guid domainId)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_roleByDomainId @p0", new object[] { domainId }).ToListAsync();
            return await db.Set<Role>().Include(x => x.Domain)
                .Where(p => p.DomainId == domainId)
                .Select(d => new RoleModel
                {
                    domainName = d.Domain.LcdaName,
                    Id = d.Id,
                    RoleName = d.RoleName,
                    RoleStatus = d.RoleStatus
                }).ToListAsync();
        }

        public async Task<List<RoleModel>> AllRoleByUserId(Guid id)
        {
            //return await db.Set<RoleExtensionModel>().FromSql("sp_getDomainRolesByUserId @p0", new object[] { id }).ToListAsync();
            return await db.Set<UserRole>()
                .Include(x => x.Role)
                .ThenInclude(x => x.Domain)
                .Where(p => p.UserId == id).Select(d => new RoleModel
                {
                    domainName = d.Role.Domain.LcdaName,
                    Id = d.RoleId,
                    RoleName = d.Role.RoleName,
                    RoleStatus = d.Role.RoleStatus
                }).ToListAsync();
        }

        public async Task<List<RoleModel>> AllRoleByUsername(string username)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_getUserDomainRoleByUsername @p0", new object[] { username }).ToListAsync();
            //return await db.Set<RoleExtensionModel>().FromSql("sp_getDomainRolesByUserId @p0", new object[] { id }).ToListAsync();
            return await db.Set<UserRole>()
                .Include(x => x.Role)
                .ThenInclude(x => x.Domain)
                .Where(p => p.User.Username == username).Select(d => new RoleModel
                {
                    domainName = d.Role.Domain.LcdaName,
                    Id = d.RoleId,
                    RoleName = d.Role.RoleName,
                    RoleStatus = d.Role.RoleStatus
                }).ToListAsync();
        }

        public async Task<List<RoleModel>> AllDomainRolesByUsername(string username)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_getDomainRolesByUsername @p0", new object[] { username }).ToListAsync();

            return await db.Set<UserRole>()
                .Include(x => x.Role)
                .ThenInclude(x => x.Domain)
                .Where(p => p.User.Username == username).Select(d => new RoleModel
                {
                    domainName = d.Role.Domain.LcdaName,
                    Id = d.RoleId,
                    RoleName = d.Role.RoleName,
                    RoleStatus = d.Role.RoleStatus
                }).ToListAsync();
        }

        public async Task<RoleModel> UserDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            //return await db.Set<RoleExtensionModel>()
            //    .FromSql("sp_getUserDomainRolesByDomainId @p0, @p1", new object[] { userid, domainid }).FirstOrDefaultAsync();
            return await db.Set<UserRole>()
                .Include(x => x.Role)
                .ThenInclude(x => x.Domain)
                .Where(p => p.UserId == userid && p.Role.DomainId == domainid).Select(d => new RoleModel
                {
                    domainName = d.Role.Domain.LcdaName,
                    Id = d.RoleId,
                    RoleName = d.Role.RoleName,
                    RoleStatus = d.Role.RoleStatus
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(UserRoleModel userRole)
        {
            db.Set<UserRole>()
                .Add(new UserRole()
                {
                    RoleId = userRole.RoleId,
                    UserId = userRole.UserId
                });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<UserRoleModel> GetUserRoleAsync(Guid userId, Guid roleId)
        {
            var r = await db.Set<UserRole>()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            if (r == null)
            {
                return null;
            }

            return new UserRoleModel()
            {
                RoleId = r.RoleId,
                UserId = r.UserId
            };
        }

        public async Task<bool> Remove(UserRoleModel userRole)
        {
            var r = await db.Set<UserRole>()
                .FirstOrDefaultAsync(x => x.UserId == userRole.UserId && x.RoleId == userRole.RoleId);

            db.Set<UserRole>().Remove(r);

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
