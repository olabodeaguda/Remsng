﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class PermissionRepository : AbstractRepository
    {
        public PermissionRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Permission>> byRoleId(Guid roleId)
        {
            return await db.Permissions.FromSql("sp_getRolePermission @p0", new object[] { roleId }).ToListAsync();
        }

        public async Task<int> PermissionCountByRoleId(Guid id)
        {
            return await db.RolePermissions.Where(x => x.RoleId == id).CountAsync();
        }

        public async Task<List<Permission>> byRoleId(Guid roleId, PageModel pageModel)
        {
            return await db.Permissions.FromSql("sp_getRolePermissionPaginated @p0, @p1, @p2", new object[] {
                roleId,
                pageModel.PageNum,
                pageModel.PageSize
            }).ToListAsync();
        }

        public async Task<List<Permission>> GetPermissionNotInRole(Guid roleId)
        {
            return await db.Permissions.FromSql("sp_getPermissionNotInRole @p0", new object[] {
                roleId
            }).ToListAsync();
        }

        public async Task<List<Permission>> All()
        {
            return await db.Permissions.ToListAsync();
        }

        public async Task<RolePermission> ByPermissionAndRoleId(RolePermission rolePermission)
        {
            return await db.RolePermissions.FirstOrDefaultAsync(x => x.PermissionId == rolePermission.PermissionId
            && x.RoleId == rolePermission.RoleId);
        }

        public async Task<bool> RemovePermission(RolePermission rolePermission)
        {
            db.RolePermissions.Remove(rolePermission);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

    }
}