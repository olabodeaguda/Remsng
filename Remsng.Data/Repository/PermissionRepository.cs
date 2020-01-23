using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DbContext db;
        public PermissionRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId)
        {
            var result = await db.Set<Permission>()
                .FromSql("sp_getRolePermission @p0", new object[] { roleId }).ToListAsync();

            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();
        }

        public async Task<int> PermissionCountByRoleId(Guid id)
        {
            return await db.Set<RolePermission>()
                .Where(x => x.RoleId == id).CountAsync();
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId, PageModel pageModel)
        {
            var result = await db.Set<Permission>()
                .FromSql("sp_getRolePermissionPaginated @p0, @p1, @p2", new object[] {
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
            string query = $"select tbl_permission.* from tbl_permission " +
                $"where id not in(select permissionId from tbl_rolePermission where roleId = '{roleId}') " +
                $"order by tbl_permission.permissionName";
            var result = await db.Set<Permission>()
                .FromSql(query).ToListAsync();

            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();

        }

        public async Task<List<PermissionModel>> All()
        {
            var result = await db.Set<Permission>().
                ToListAsync();
            return result.Select(x => new PermissionModel()
            {
                Id = x.Id,
                PermissionDescription = x.PermissionDescription,
                PermissionName = x.PermissionName
            }).ToList();
        }

        public async Task<RolePermissionModel> ByPermissionAndRoleId(RolePermissionModel rolePermission)
        {
            var result = await db.Set<RolePermission>()
                .FirstOrDefaultAsync(x => x.PermissionId == rolePermission.PermissionId
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
            var result = await db.Set<RolePermission>()
               .FirstOrDefaultAsync(x => x.PermissionId == rolePermission.PermissionId
           && x.RoleId == rolePermission.RoleId);

            db.Set<RolePermission>()
                .Remove(result);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

    }
}
