using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IRoleService
    {
        Task<bool> UpdateStatus(Role role);
        Task<bool> Update(Role role);
        Task<RoleExtension> GetById(Guid id);
        Task<RoleExtension> GetByName(string rolename);
        Task<bool> Add(Role role);
        Task<RoleExtension> GetUserDomainRoleByUsername(string username, Guid domainId);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<bool> Add(RolePermission role);
        Task<List<RoleExtension>> ByDomainId(Guid domainId);
        Task<List<RoleExtension>> AllRoleByUsername(string username);
        Task<List<RoleExtension>> AllDomainRolesByUsername(string username);
        Task<bool> AssignRoleToUserAsync(UserRole userRole);
        Task<UserRole> GetUserRoleAsync(Guid userId, Guid roleId);
        Task<RoleExtension> UserDomainRolesByDomainId(Guid userid, Guid domainid);
        Task<List<RoleExtension>> AllRoleByUserId(Guid id);
        Task<bool> Remove(UserRole userRole);
        Task<object> Paginated(Models.PageModel pageModel, Guid domainId);
    }
}
