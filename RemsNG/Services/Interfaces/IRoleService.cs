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
        Task<Role> GetById(Guid id);
        Task<Role> GetByName(string rolename);
        Task<bool> Add(Role role);

    }
}
