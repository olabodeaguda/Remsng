using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class PermissionManager : IPermissionManager
    {
        private readonly IPermissionRepository permissionDao;
        public PermissionManager(IPermissionRepository permissionRepository)
        {
            permissionDao = permissionRepository;
        }

        public async Task<List<PermissionModel>> All()
        {
            return await permissionDao.All();
        }

        public async Task<RolePermissionModel> ByPermissionAndRoleId(RolePermissionModel rolePermission)
        {
            return await permissionDao.ByPermissionAndRoleId(rolePermission);
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId)
        {
            return await permissionDao.byRoleId(roleId);
        }

        public async Task<List<PermissionModel>> byRoleId(Guid roleId, PageModel pageModel)
        {
            return await permissionDao.byRoleId(roleId, pageModel);
        }

        public async Task<List<PermissionModel>> GetPermissionNotInRole(Guid roleId)
        {
            return await permissionDao.GetPermissionNotInRole(roleId);
        }

        public async Task<int> PermissionCountByRoleId(Guid id)
        {
            return await permissionDao.PermissionCountByRoleId(id);
        }

        public async Task<bool> RemovePermission(RolePermissionModel rolePermission)
        {
            return await permissionDao.RemovePermission(rolePermission);
        }
    }
}
