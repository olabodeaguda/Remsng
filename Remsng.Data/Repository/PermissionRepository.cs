using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class PermissionRepository : AbstractRepository
    {
        public PermissionRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId)
        {
            var result = await db.Permissions.FromSql("sp_getRolePermission @p0", new object[] { roleId }).ToListAsync();

            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();
        }

        public async Task<int> PermissionCountByRoleId(Guid id)
        {
            return await db.RolePermissions.Where(x => x.RoleId == id).CountAsync();
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId, PageModel pageModel)
        {
            var result = await db.Permissions.FromSql("sp_getRolePermissionPaginated @p0, @p1, @p2", new object[] {
                roleId,
                pageModel.PageNum,
                pageModel.PageSize
            }).ToListAsync();

            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();

        }

        public async Task<List<PermissionModel>> GetPermissionNotInRole(Guid roleId)
        {
            var result = await db.Permissions.FromSql("sp_getPermissionNotInRole @p0", new object[] {
                roleId
            }).ToListAsync();

            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();

        }

        public async Task<List<PermissionModel>> All()
        {
            var result = await db.Permissions.ToListAsync();
            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();
        }

        public async Task<RolePermissionModel> ByPermissionAndRoleId(RolePermissionModel rolePermission)
        {
            var result = await db.RolePermissions.FirstOrDefaultAsync(x => x.PermissionId == rolePermission.PermissionId
            && x.RoleId == rolePermission.RoleId);

            if (result == null)
            {
                return null;
            }

            return new RolePermissionModel()
            {
                PermissionId = result.PermissionId,
                RoleId = result.RoleId
            };

        }

        public async Task<bool> RemovePermission(RolePermissionModel rolePermission)
        {
            db.RolePermissions.Remove(new RolePermission()
            {
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

    }
}
