using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel> Get(Guid id);
        Task<bool> Update(UserModel user);
        Task<bool> Create(UserModel result);
        Task<bool> AddAndAssignLGDA(UserModel result, UserLcdaModel userLcda);
        Task<bool> AssignLGDA(UserLcdaModel userLcda);
        Task<UserModel> ByEmail(string email);
        Task<UserModel> ByUserName(string username);
        Task<object> Paginated(PageModel pageModel);
        Task<object> Paginated(PageModel pageModel, Guid lcdaId);
        Task<bool> ChangePwd(Guid id, string newPwd);
        Task<bool> ChangeStatus(string status, Guid id);
    }
}
