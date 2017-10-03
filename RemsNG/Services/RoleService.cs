using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class RoleService : IRoleService
    {
        RoleDao roleDao;
        public RoleService(RemsDbContext _db)
        {
            roleDao = new RoleDao(_db);
        }

        public async Task<bool> Add(Role role)
        {
            return await roleDao.Add(role);
        }

        public async Task<bool> Add(RolePermission role)
        {
            return await roleDao.Add(role);
        }

        public async Task<List<RoleExtension>> AllDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            return await roleDao.AllDomainRolesByDomainId(userid, domainid);
        }

        public async Task<List<RoleExtension>> AllDomainRolesByUsername(string username)
        {
            return await roleDao.AllDomainRolesByUsername(username);
        }

        public async Task<List<RoleExtension>> AllRoleByUsername(string username)
        {
            return await roleDao.AllRoleByUsername(username);
        }

        public async Task<bool> AssignRoleToUserAsync(UserRole userRole)
        {
            return await roleDao.AssignRoleToUserAsync(userRole);
        }

        public async Task<List<Role>> ByDomainId(Guid domainId)
        {
            return await roleDao.ByDomainId(domainId);
        }

        public async Task<RoleExtension> GetById(Guid id)
        {
            return await roleDao.GetById(id);
        }

        public async Task<Role> GetByName(string rolename)
        {
            return await roleDao.GetByName(rolename);
        }

        public async Task<RoleExtension> GetUserDomainRoleByUsername(string username, Guid domainId)
        {
            return await roleDao.GetUserDomainRoleByUsername(username, domainId);
        }

        public async Task<UserRole> GetUserRoleAsync(Guid userId, Guid roleId)
        {
            return await roleDao.GetUserRoleAsync(userId, roleId);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await roleDao.Paginated(pageModel);
        }

        public async Task<bool> Update(Role role)
        {
            return await roleDao.Update(role);
        }

        public async Task<bool> UpdateStatus(Role role)
        {
            return await roleDao.UpdateStatus(role);
        }
    }
}
