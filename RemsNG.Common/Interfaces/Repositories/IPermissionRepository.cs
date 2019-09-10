using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<List<PermissionModel>> byRoleId(Guid roleId);
        Task<int> PermissionCountByRoleId(Guid id);
        Task<List<PermissionModel>> byRoleId(Guid roleId, PageModel pageModel);
        Task<List<PermissionModel>> GetPermissionNotInRole(Guid roleId);
        Task<List<PermissionModel>> All();
        Task<RolePermissionModel> ByPermissionAndRoleId(RolePermissionModel rolePermission);
        Task<bool> RemovePermission(RolePermissionModel rolePermission);
    }
}
