using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IPermission
    {
        Task<List<Permission>> byRoleId(Guid roleId);
        Task<List<Permission>> byRoleId(Guid roleId, PageModel pageModel);
        Task<List<Permission>> All();
        Task<List<Permission>> GetPermissionNotInRole(Guid roleId);
        Task<RolePermission> ByPermissionAndRoleId(RolePermission rolePermission);
        Task<bool> RemovePermission(RolePermission rolePermission);
        Task<int> PermissionCountByRoleId(Guid id);
    }
}
