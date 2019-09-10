using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<RoleModel> GetUserDomainRoleByUsername(string username, Guid domainId);
        Task<object> Paginated(PageModel pageModel);
        Task<object> Paginated(PageModel pageModel, Guid domainId);
        Task<bool> Add(RoleModel role);
        Task<bool> Add(RolePermissionModel role);
        Task<RoleModel> GetByName(string rolename);
        Task<RoleModel> GetById(Guid id);
        Task<bool> Update(RoleModel role);
        Task<bool> UpdateStatus(RoleModel role);
        Task<List<RoleModel>> ByDomainId(Guid domainId);
        Task<List<RoleModel>> AllRoleByUserId(Guid id);
        Task<List<RoleModel>> AllRoleByUsername(string username);
        Task<List<RoleModel>> AllDomainRolesByUsername(string username);
        Task<RoleModel> UserDomainRolesByDomainId(Guid userid, Guid domainid);
        Task<bool> AssignRoleToUserAsync(UserRoleModel userRole);
        Task<UserRoleModel> GetUserRoleAsync(Guid userId, Guid roleId);
        Task<bool> Remove(UserRoleModel userRole);
    }
}
