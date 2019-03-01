using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IRoleManager
    {
        Task<bool> UpdateStatus(RoleModel role);
        Task<bool> Update(RoleModel role);
        Task<RoleModel> GetById(Guid id);
        Task<RoleModel> GetByName(string rolename);
        Task<bool> Add(RoleModel role);
        Task<RoleModel> GetUserDomainRoleByUsername(string username, Guid domainId);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<bool> Add(RolePermissionModel role);
        Task<List<RoleModel>> ByDomainId(Guid domainId);
        Task<List<RoleModel>> AllRoleByUsername(string username);
        Task<List<RoleModel>> AllDomainRolesByUsername(string username);
        Task<bool> AssignRoleToUserAsync(UserRoleModel userRole);
        Task<UserRoleModel> GetUserRoleAsync(Guid userId, Guid roleId);
        Task<RoleModel> UserDomainRolesByDomainId(Guid userid, Guid domainid);
        Task<List<RoleModel>> AllRoleByUserId(Guid id);
        Task<bool> Remove(UserRoleModel userRole);
        Task<object> Paginated(Models.PageModel pageModel, Guid domainId);
    }
}
