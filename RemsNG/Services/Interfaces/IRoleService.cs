using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IRoleService
    {
        Task<bool> UpdateStatus(Role role);
        Task<bool> Update(Role role);
        Task<RoleExtension> GetById(Guid id);
        Task<Role> GetByName(string rolename);
        Task<bool> Add(Role role);
        Task<RoleExtension> GetUserDomainRoleByUsername(string username, Guid domainId);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<bool> Add(RolePermission role);
        Task<List<Role>> ByDomainId(Guid domainId);
        Task<List<RoleExtension>> AllRoleByUsername(string username);
        Task<List<RoleExtension>> AllDomainRolesByUsername(string username);
        Task<bool> AssignRoleToUserAsync(UserRole userRole);
        Task<UserRole> GetUserRoleAsync(Guid userId, Guid roleId);
        Task<List<RoleExtension>> AllDomainRolesByDomainId(Guid userid, Guid domainid);
    }
}
