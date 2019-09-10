﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class SectorManager : ISectorManager
    {
        private readonly ISectorRepository sectorDao;
        public SectorManager(ISectorRepository sectorRepository)
        {
            sectorDao = sectorRepository;
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
