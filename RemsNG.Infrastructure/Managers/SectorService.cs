using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using Microsoft.Extensions.Logging;
using RemsNG.Dao;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Data.Repository;
using Remsng.Data;
using RemsNG.Common.Models;

namespace RemsNG.Services
{
    public class SectorManagers : ISectorManagers
    {
        SectorRepository sectorDao;
        public SectorManagers(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            sectorDao = new SectorRepository(_db, loggerFactory);
        }
        public async Task<Response> Add(SectorModel sector)
        {
            return await sectorDao.Add(sector);
        }

        public async Task<SectorModel> ById(Guid id)
        {
            return await sectorDao.ById(id);
        }

        public async Task<List<SectorModel>> ByLcdaId(Guid lcdaId)
        {
            return await sectorDao.ByLcdaId(lcdaId);
        }

        public async Task<SectorModel> ByTaxpayerId(Guid taxpayerId)
        {
            return await sectorDao.ByTaxpayerId(taxpayerId);
        }

        public async Task<Response> Update(SectorModel sector)
        {
            return await sectorDao.Update(sector);
        }
    }
}
