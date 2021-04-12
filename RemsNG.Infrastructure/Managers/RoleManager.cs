using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleRepository roleDao;
        public RoleManager(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
            roleDao = roleRepository;
        }

        public async Task<bool> Add(RoleModel role)
        {
            return await roleDao.Add(role);
        }

        public async Task<bool> Add(RolePermissionModel role)
        {
            return await roleDao.Add(role);
        }

        public async Task<RoleModel> UserDomainRolesByDomainId(Guid userid, Guid domainid)
        {
            return await roleDao.UserDomainRolesByDomainId(userid, domainid);
        }

        public async Task<List<RoleModel>> AllDomainRolesByUsername(string username)
        {
            return await roleDao.AllDomainRolesByUsername(username);
        }

        public async Task<List<RoleModel>> AllRoleByUserId(Guid id)
        {
            return await roleDao.AllRoleByUserId(id);
        }

        public async Task<List<RoleModel>> AllRoleByUsername(string username)
        {
            return await roleDao.AllRoleByUsername(username);
        }

        public async Task<bool> AssignRoleToUserAsync(UserRoleModel userRole)
        {
            return await roleDao.AssignRoleToUserAsync(userRole);
        }

        public async Task<List<RoleModel>> ByDomainId(Guid domainId)
        {
            return await roleDao.ByDomainId(domainId);
        }

        public async Task<RoleModel> GetById(Guid id)
        {
            return await roleDao.GetById(id);
        }

        public async Task<RoleModel> GetByName(string rolename)
        {
            return await roleDao.GetByName(rolename);
        }

        public async Task<RoleModel> GetUserDomainRoleByUsername(string username, Guid domainId)
        {
            return await roleDao.GetUserDomainRoleByUsername(username, domainId);
        }

        public async Task<UserRoleModel> GetUserRoleAsync(Guid userId, Guid roleId)
        {
            return await roleDao.GetUserRoleAsync(userId, roleId);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await roleDao.Paginated(pageModel);
        }

        public async Task<bool> Remove(UserRoleModel userRole)
        {
            return await roleDao.Remove(userRole);
        }

        public async Task<bool> Update(RoleModel role)
        {
            return await roleDao.Update(role);
        }

        public async Task<bool> UpdateStatus(RoleModel role)
        {
            return await roleDao.UpdateStatus(role);
        }

        public async Task<object> Paginated(PageModel pageModel, Guid domainId)
        {
            return await roleDao.Paginated(pageModel, domainId);
        }

        public async Task<bool> IsPermited(string userId, string permissionName)
        {
            return await _permissionRepository.ByUserIdAndPermissionName(userId, permissionName);
        }
    }
}
