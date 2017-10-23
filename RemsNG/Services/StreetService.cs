using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using Microsoft.Extensions.Logging;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class StreetService : IStreetService
    {
        StreetDao streetDao;
        public StreetService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            streetDao = new StreetDao(_db, loggerFactory);
        }
        public async Task<Response> Add(Street street)
        {
            return await streetDao.Add(street);
        }

        public async Task<Street> ById(Guid streetId)
        {
            return await streetDao.ById(streetId);
        }

        public async Task<List<Street>> ByLcda(Guid lcdaId)
        {
            return await streetDao.ByLcda(lcdaId);
        }

        public async Task<List<Street>> ByWard(Guid wardId)
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

        public async Task<Response> Update(Street street)
        {
            return await streetDao.Update(street);
        }
    }
}
