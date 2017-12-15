using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface ILcdaService
    {
        Task<object> All();
        Task<object> All(PageModel pageModel);
        Task<List<Lgda>> ActiveLCDAByDomainId(Guid domainId);
        Task<bool> Add(Lgda lcda);
        Task<bool> Update(Lgda lcda);
        Task<Lgda> Get(Guid id);
        Task<bool> Changetatus(Guid id, string lcdastatus);
        Task<Lgda> byLCDACode(string lcdaCode);
        Task<List<Lgda>> byUsername(string username);
        Task<UserLcda> UserLcdaByIds(Guid lgdaId, Guid userId);
        Task<List<Lgda>> UserDomainByUserId(Guid id);
        Task<List<Lgda>> UserRoleDomainbyUserId(Guid id);
        Task<List<Lgda>> UnAssignUserDomainByUserId(Guid userid);
        Task<bool> RemoveUserFromLCDA(UserLcda userLcda);
        Task<Lgda> ByStreet(Guid streetId);
        Task<Domain> GetDomain(Guid lcdaId);
        Task<Lgda> Get(DemandNoticeRequest dnr);
        Task<Lgda> GetLcdaExtension(Guid lcdaId);
        Task<Lgda> ByBillingNumber(String billingno);
    }
}
