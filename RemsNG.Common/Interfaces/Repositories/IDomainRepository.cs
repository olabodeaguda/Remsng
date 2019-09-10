using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDomainRepository
    {
        Task<List<DomainModel>> ActiveDomains();
        Task<object> Paginated(PageModel pageModel);
        Task<bool> Add(DomainModel domain);
        Task<List<DomainModel>> GetUserDomainByUsername(string username);
        Task<List<DomainModel>> GetUserDomainByUsernameId(Guid id);
        Task<DomainModel> byDomainId(Guid id);
        Task<DomainModel> DomainbyLCDAId(Guid lcdaId);
        Task<DomainModel> byDomainCode(string domainCode);
        Task<bool> UpdateDomain(DomainModel domain);
        Task<bool> changeDomain(Guid domainId, string domainStatus);
    }
}
