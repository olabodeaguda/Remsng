using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Models;

namespace RemsNG.Services
{
    public class RoleService : IRoleService
    {
        RoleDao roleDao;
        public RoleService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            roleDao = new RoleDao(_db,loggerFactory);
        }

        public async Task<bool> Add(Role role)
        {
            return await roleDao.Add(role);
        }

        public async Task<bool> Add(RolePermission role)
        {
            return await roleDao.Add(role);
        }

        public async Task<RoleExtension> UserDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            return await roleDao.UserDomainRolesByDomainId(userid, domainid);
        }

        public async Task<List<RoleExtension>> AllDomainRolesByUsername(string username)
        {
            return await roleDao.AllDomainRolesByUsername(username);
        }

        public async Task<List<RoleExtension>> AllRoleByUserId(Guid id)
        {
            return await roleDao.AllRoleByUserId(id);
        }

        public async Task<List<RoleExtension>> AllRoleByUsername(string username)
        {
            return await roleDao.AllRoleByUsername(username);
        }

        public async Task<bool> AssignRoleToUserAsync(UserRole userRole)
        {
            return await roleDao.AssignRoleToUserAsync(userRole);
        }

        public async Task<List<RoleExtension>> ByDomainId(Guid domainId)
        {
            return await roleDao.ByDomainId(domainId);
        }

        public async Task<RoleExtension> GetById(Guid id)
        {
            return await roleDao.GetById(id);
        }

        public async Task<RoleExtension> GetByName(string rolename)
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

        public async Task<bool> Remove(UserRole userRole)
        {
            return await roleDao.Remove(userRole);
        }

        public async Task<bool> Update(Role role)
        {
            return await roleDao.Update(role);
        }

        public async Task<bool> UpdateStatus(Role role)
        {
            return await roleDao.UpdateStatus(role);
        }

        public async Task<object> Paginated(PageModel pageModel, Guid domainId)
        {
            return await roleDao.Paginated(pageModel, domainId);
        }
    }
}
