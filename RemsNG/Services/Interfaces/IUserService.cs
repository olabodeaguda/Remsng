using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Get(Guid id);
        Task<User> GetUserByUsername(string username);
        Task<User> ByEmail(string email);
        Task<object> GetToken(User user, Guid domainId);
        Task<bool> Create(User user);
        Task<bool> AddAndAssignLGDA(User user, UserLcda userLcda);
        Task<object> Paginated(Models.PageModel pageModel, Guid lcdaId);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<bool> Update(User user);
        Task<bool> ChangePwd(Guid id, string newPwd);
        Task<bool> AssignLGDA(UserLcda userLcda);
    }
}
