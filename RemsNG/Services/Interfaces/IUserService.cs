using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByUsername(string username);
        Task<string> GetToken(User user, Guid domainId, bool byDomain);
    }
}
