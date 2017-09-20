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
        Task<object> AllPaginated(PageModel pageModel);
        Task<List<Lcda>> ActiveLCDAByDomainId(Guid domainId);
        Task<bool> Add(Lcda lcda);
        Task<bool> Update(Lcda lcda);
        Task<Lcda> Get(Guid id);
        Task<bool> Changetatus(Guid id, string lcdastatus);
        Task<Lcda> byLCDACode(string lcdaCode);
        Task<List<UserLcda>> byUsername(string username);
    }
}
