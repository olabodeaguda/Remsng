using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ILcdaRepository
    {
        Task<LcdaModel> Get(Guid id);
        Task<List<LcdaModel>> All();
        Task<object> All(PageModel pageModel);
        Task<List<LcdaModel>> ActiveLCDAByDomainId(Guid domainId);
        Task<UserLcdaModel> UserLcdaByIds(Guid lgdaId, Guid userId);
        Task<bool> Add(LcdaModel lcda);
        Task<bool> Update(LcdaModel lcda);
        Task<bool> Changetatus(Guid id, string lcdastatus);
        Task<LcdaModel> byLcdaCode(string lcdaCode);
        Task<List<LcdaModel>> getLcdaByUsername(string username);
        Task<List<LcdaModel>> UserDomainByUserId(Guid id);
        Task<List<LcdaModel>> unAssignUserDomainByUserId(Guid userid);
        Task<bool> RemoveUserFromLCDA(UserLcdaModel userLcda);
        Task<LcdaModel> ByStreet(Guid streetId);
        Task<DomainModel> GetDomain(Guid lcdaId);
        Task<LcdaModel> GetLcdaExtension(Guid lcdaId);
        Task<LcdaModel> ByBillingNumber(long billingno);
    }
}
