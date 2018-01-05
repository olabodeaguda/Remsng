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
    public class SectorService : ISectorService
    {
        SectorDao sectorDao;
        public SectorService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            sectorDao = new SectorDao(_db, loggerFactory);
        }
        public async Task<Response> Add(Sector sector)
        {
            return await sectorDao.Add(sector);
        }

        public async Task<Sector> ById(Guid id)
        {
            return await sectorDao.ById(id);
        }

        public async Task<List<Sector>> ByLcdaId(Guid lcdaId)
        {
            return await sectorDao.ByLcdaId(lcdaId);
        }

        public async Task<Sector> ByTaxpayerId(Guid taxpayerId)
        {
            return await sectorDao.ByTaxpayerId(taxpayerId);
        }

        public async Task<Response> Update(Sector sector)
        {
            return await sectorDao.Update(sector);
        }
    }
}
