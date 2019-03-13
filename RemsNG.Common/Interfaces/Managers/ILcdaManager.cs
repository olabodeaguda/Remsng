using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ILcdaManager
    {
        Task<object> All();
        Task<object> All(PageModel pageModel);
        Task<List<LcdaModel>> ActiveLcdaByDomainId(Guid domainId);
        Task<bool> Add(LcdaModel LcdaModel);
        Task<bool> Update(LcdaModel LcdaModel);
        Task<LcdaModel> Get(Guid id);
        Task<bool> Changetatus(Guid id, string LcdaModelstatus);
        Task<LcdaModel> ByLcdaCode(string LcdaModelCode);
        Task<List<LcdaModel>> byUsername(string username);
        Task<UserLcdaModel> UserLcdaByIds(Guid LcdaModelId, Guid userId);
        Task<List<LcdaModel>> UserDomainByUserId(Guid id);
        Task<List<LcdaModel>> UserRoleDomainbyUserId(Guid id);
        Task<List<LcdaModel>> UnAssignUserDomainByUserId(Guid userid);
        Task<bool> RemoveUserFromLcda(UserLcdaModel userLcdaModel);
        Task<LcdaModel> ByStreet(Guid streetId);
        Task<DomainModel> GetDomain(Guid LcdaModelId);
        Task<LcdaModel> Get(DemandNoticeRequestModel dnr);
        Task<LcdaModel> GetLcdaExtension(Guid LcdaModelId);
        Task<LcdaModel> ByBillingNumber(long billingno);
    }
}
