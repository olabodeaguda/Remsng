using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

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

        public async Task<Role> GetById(Guid id)
        {
            return await roleDao.GetById(id);
        }

        public async Task<Role> GetByName(string rolename)
        {
            return await roleDao.GetByName(rolename);
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
