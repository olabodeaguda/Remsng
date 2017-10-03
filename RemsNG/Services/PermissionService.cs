﻿using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class PermissionService : IPermission
    {
        PermissionDao permissionDao;
        public PermissionService(RemsDbContext _db)
        {
            permissionDao = new PermissionDao(_db);
        }

        public async Task<List<Permission>> All()
        {
            return await permissionDao.All();
        }

        public async Task<RolePermission> ByPermissionAndRoleId(RolePermission rolePermission)
        {
            return await permissionDao.ByPermissionAndRoleId(rolePermission);
        }

        public async Task<List<Permission>> byRoleId(Guid roleId)
        {
            return await permissionDao.byRoleId(roleId);
        }

        public async Task<List<Permission>> byRoleId(Guid roleId, PageModel pageModel)
        {
            return await permissionDao.byRoleId(roleId, pageModel);
        }

        public async Task<List<Permission>> GetPermissionNotInRole(Guid roleId)
        {
            return await permissionDao.GetPermissionNotInRole(roleId);
        }

        public async Task<bool> RemovePermission(RolePermission rolePermission)
        {
            return await permissionDao.RemovePermission(rolePermission);
        }
    }
}