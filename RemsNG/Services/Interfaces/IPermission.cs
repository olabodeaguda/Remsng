using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IPermission
    {
        Task<List<Permission>> byRoleId(Guid roleId);
        Task<List<Permission>> byRoleId(Guid roleId, Models.PageModel pageModel);
        Task<List<Permission>> All();
        Task<List<Permission>> GetPermissionNotInRole(Guid roleId);
        Task<RolePermission> ByPermissionAndRoleId(RolePermission rolePermission);
        Task<bool> RemovePermission(RolePermission rolePermission);
        Task<int> PermissionCountByRoleId(Guid id);
    }
}
