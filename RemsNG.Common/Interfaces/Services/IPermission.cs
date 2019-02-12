using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IPermission
    {
        Task<List<PermissionModel>> byRoleId(Guid roleId);
        Task<List<PermissionModel>> byRoleId(Guid roleId, PageModel pageModel);
        Task<List<PermissionModel>> All();
        Task<List<PermissionModel>> GetPermissionNotInRole(Guid roleId);
        Task<RolePermissionModel> ByPermissionAndRoleId(RolePermissionModel rolePermission);
        Task<bool> RemovePermission(RolePermissionModel rolePermission);
        Task<int> PermissionCountByRoleId(Guid id);
    }
}
