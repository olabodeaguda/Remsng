using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class StreetManagers : IStreetManagers
    {
        StreetRepository streetDao;
        public StreetManagers(DbContext _db, ILoggerFactory loggerFactory)
        {
            streetDao = new StreetRepository(_db, loggerFactory);
        }
        public async Task<Response> Add(StreetModel street)
        {
            return await streetDao.Add(street);
        }

        public async Task<StreetModel> ById(Guid streetId)
        {
            return await streetDao.ById(streetId);
        }

        public async Task<List<StreetModel>> ByLcda(Guid lcdaId)
        {
            return await streetDao.ByLcda(lcdaId);
        }

        public async Task<List<StreetModel>> ByWard(Guid wardId)
        {
            return await streetDao.ByWard(wardId);
        }

        public async Task<int> ByWardCount(Guid wardId)
        {
            return await streetDao.ByWardCount(wardId);
        }

        public Task<object> ByWardpaginated(Guid wardId, PageModel pageModel)
        {
            return streetDao.ByWardpaginated(wardId, pageModel);
        }

        public async Task<Response> ChangeStatus(Guid id, string streetStatus)
        {
            return await streetDao.ChangeStatus(id, streetStatus);
        }

        public async Task<DomainModel> GetDomain(Guid streetId)
        {
            return await streetDao.GetDomain(streetId);
        }

        public async Task<Response> Update(StreetModel street)
        {
            return await streetDao.Update(street);
        }

        public async Task<List<StreetModel>> Search(Guid wardId, string searchName)
        {
            return await streetDao.SearchStreet(wardId, searchName);
        }

    }
}
