using RemsNG.Common.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IUserManagers
    {
        Task<UserModel> Get(Guid id);
        Task<UserModel> GetUserByUsername(string username);
        Task<UserModel> ByEmail(string email);
        Task<object> GetToken(UserModel user, Guid domainId);
        string GetToken(Claim[] claim);
        Task<bool> Create(UserModel user);
        Task<bool> AddAndAssignLGDA(UserModel user, UserLcdaModel userLcda);
        Task<object> Paginated(PageModel pageModel, Guid lcdaId);
        Task<object> Paginated(PageModel pageModel);
        Task<bool> Update(UserModel user);
        Task<bool> ChangePwd(Guid id, string newPwd);
        Task<bool> AssignLGDA(UserLcdaModel userLcda);
        Task<bool> ChangeStatus(string status, Guid id);
    }
}
