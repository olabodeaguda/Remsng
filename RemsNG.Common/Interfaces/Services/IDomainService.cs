using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDomainService
    {
        Task<List<DomainModel>> ActiveDomains();
        Task<object> Paginated(PageModel pageModel);
        Task<List<DomainModel>> GetDomainByUsername(string username);
        Task<bool> Add(DomainModel domain);
        Task<DomainModel> ByDomainCode(string domainCode);
        Task<DomainModel> ByDomainId(Guid domainId);
        Task<bool> UpdateDomain(DomainModel domain);
        Task<bool> ChangeDomain(Guid domainId, string domainStatus);
        Task<List<DomainModel>> GetUserDomainByUsernameId(Guid id);
        Task<DomainModel> DomainbyLCDAId(Guid lcdaId);
    }
}
