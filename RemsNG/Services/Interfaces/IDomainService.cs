using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
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
    }
}
