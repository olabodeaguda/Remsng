﻿using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class DomainManagers : IDomainManagers
    {
        private readonly DomainRepository domainDao;

        public DomainManagers(RemsDbContext _db)
        {
            domainDao = new DomainRepository(_db);
        }

        public async Task<bool> Add(DomainModel domain)
        {
            return await domainDao.Add(domain);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await domainDao.Paginated(pageModel);
        }

        public async Task<DomainModel> ByDomainCode(string domainCode)
        {
            return await domainDao.byDomainCode(domainCode);
        }

        public async Task<DomainModel> ByDomainId(Guid domainId)
        {
            return await domainDao.byDomainId(domainId);
        }

        public async Task<List<DomainModel>> GetDomainByUsername(string username)
        {
            return await domainDao.GetUserDomainByUsername(username);
        }

        public Task<bool> UpdateDomain(DomainModel domain)
        {
            return domainDao.UpdateDomain(domain);
        }

        public Task<bool> ChangeDomain(Guid domainId, string domainStatus)
        {
            return domainDao.changeDomain(domainId, domainStatus);
        }

        public async Task<List<DomainModel>> ActiveDomains()
        {
            return await domainDao.ActiveDomains();
        }

        public async Task<List<DomainModel>> GetUserDomainByUsernameId(Guid id)
        {
            return await domainDao.GetUserDomainByUsernameId(id);
        }

        public async Task<DomainModel> DomainbyLCDAId(Guid lcdaId)
        {
            return await domainDao.DomainbyLCDAId(lcdaId);
        }

    }
}