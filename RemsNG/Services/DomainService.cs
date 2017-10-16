using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class DomainService : IDomainService
    {
        private readonly DomainDao domainDao;

        public DomainService(RemsDbContext _db)
        {
            domainDao = new DomainDao(_db);
        }

        public async Task<bool> Add(Domain domain)
        {
            return await domainDao.Add(domain);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await domainDao.Paginated(pageModel);
        }

        public async Task<Domain> ByDomainCode(string domainCode)
        {
            return await domainDao.byDomainCode(domainCode);
        }

        public async Task<Domain> ByDomainId(Guid domainId)
        {
            return await domainDao.byDomainId(domainId);
        }


        public async Task<List<Domain>> GetDomainByUsername(string username)
        {
            return await domainDao.GetUserDomainByUsername(username);
        }

        public Task<bool> UpdateDomain(Domain domain)
        {
            return domainDao.UpdateDomain(domain);
        }

        public Task<bool> ChangeDomain(Guid domainId, string domainStatus)
        {
            return domainDao.changeDomain(domainId, domainStatus);
        }

        public async Task<List<Domain>> ActiveDomains()
        {
            return await domainDao.ActiveDomains();
        }

        public async Task<List<Domain>> GetUserDomainByUsernameId(Guid id)
        {
            return await domainDao.GetUserDomainByUsernameId(id);
        }

        public async Task<Domain> DomainbyLCDAId(Guid lcdaId)
        {
            return await domainDao.DomainbyLCDAId(lcdaId);
        }
    }
}
