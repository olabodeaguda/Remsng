using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ILcdaService
    {
        Task<object> All();
        Task<object> All(PageModel pageModel);
        Task<List<LcdaModel>> ActiveLcdaModelModelByDomainId(Guid domainId);
        Task<bool> Add(LcdaModel LcdaModelModel);
        Task<bool> Update(LcdaModel LcdaModel);
        Task<LcdaModel> Get(Guid id);
        Task<bool> Changetatus(Guid id, string LcdaModelstatus);
        Task<LcdaModel> byLcdaModelCode(string LcdaModelCode);
        Task<List<LcdaModel>> byUsername(string username);
        Task<UserLcdaModel> UserLcdaModelByIds(Guid LcdaModelId, Guid userId);
        Task<List<LcdaModel>> UserDomainByUserId(Guid id);
        Task<List<LcdaModel>> UserRoleDomainbyUserId(Guid id);
        Task<List<LcdaModel>> UnAssignUserDomainByUserId(Guid userid);
        Task<bool> RemoveUserFromLcdaModel(UserLcdaModel userLcdaModel);
        Task<LcdaModel> ByStreet(Guid streetId);
        Task<DomainModel> GetDomain(Guid LcdaModelId);
        Task<LcdaModel> Get(DemandNoticeRequestModel dnr);
        Task<LcdaModel> GetLcdaModelExtension(Guid LcdaModelId);
        Task<LcdaModel> ByBillingNumber(String billingno);
    }
}
