using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDomainService
    {
        Task<List<Domain>> ActiveDomains();
        Task<object> Paginated(PageModel pageModel);
        Task<List<Domain>> GetDomainByUsername(string username);
        Task<bool> Add(Domain domain);
        Task<Domain> ByDomainCode(string domainCode);
        Task<Domain> ByDomainId(Guid domainId);
        Task<bool> UpdateDomain(Domain domain);
        Task<bool> ChangeDomain(Guid domainId, string domainStatus);
        Task<List<Domain>> GetUserDomainByUsernameId(Guid id);
        Task<Domain> DomainbyLCDAId(Guid lcdaId);
    }
}
