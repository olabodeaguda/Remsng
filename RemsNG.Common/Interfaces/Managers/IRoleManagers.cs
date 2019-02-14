using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IRoleManagers
    {
        Task<bool> UpdateStatus(RoleModel role);
        Task<bool> Update(RoleModel role);
        Task<RoleExtensionModel> GetById(Guid id);
        Task<RoleExtensionModel> GetByName(string rolename);
        Task<bool> Add(RoleModel role);
        Task<RoleExtensionModel> GetUserDomainRoleByUsername(string username, Guid domainId);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<bool> Add(RolePermissionModel role);
        Task<List<RoleExtensionModel>> ByDomainId(Guid domainId);
        Task<List<RoleExtensionModel>> AllRoleByUsername(string username);
        Task<List<RoleExtensionModel>> AllDomainRolesByUsername(string username);
        Task<bool> AssignRoleToUserAsync(UserRoleModel userRole);
        Task<UserRoleModel> GetUserRoleAsync(Guid userId, Guid roleId);
        Task<RoleExtensionModel> UserDomainRolesByDomainId(Guid userid, Guid domainid);
        Task<List<RoleExtensionModel>> AllRoleByUserId(Guid id);
        Task<bool> Remove(UserRoleModel userRole);
        Task<object> Paginated(Models.PageModel pageModel, Guid domainId);
    }
}
