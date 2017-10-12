﻿using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class LcdaService : ILcdaService
    {
        private readonly LcdaDao lcdaDao;

        public LcdaService(RemsDbContext _db)
        {
            lcdaDao = new LcdaDao(_db);
        }

        public async Task<List<Lgda>> ActiveLCDAByDomainId(Guid domainId)
        {
            return await lcdaDao.ActiveLCDAByDomainId(domainId);
        }

        public async Task<bool> Add(Lgda lcda)
        {
            return await lcdaDao.Add(lcda);
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await lcdaDao.All(pageModel);
        }

        public async Task<object> All()
        {
            return await lcdaDao.All();
        }

        public async Task<Lgda> byLCDACode(string lcdaCode)
        {
            return await lcdaDao.byLcdaCode(lcdaCode);
        }

        public async Task<List<Lgda>> byUsername(string username)
        {
            return await lcdaDao.getLcdaByUsername(username);
        }

        public async Task<bool> Changetatus(Guid id, string lcdastatus)
        {
            return await lcdaDao.Changetatus(id, lcdastatus);
        }

        public Task<Lgda> Get(Guid id)
        {
            return lcdaDao.Get(id);
        }

        public async Task<bool> Update(Lgda lcda)
        {
            return await lcdaDao.Update(lcda);
        }

        public async Task<List<Lgda>> UserDomainByUserId(Guid id)
        {
            return await lcdaDao.UserDomainByUserId(id);
        }

        public async Task<UserLcda> UserLcdaByIds(Guid lgdaId, Guid userId)
        {
            return await lcdaDao.UserLcdaByIds(lgdaId, userId);
        }

        //public async Task<object> UserRoleDomainbyUserId(Guid id)
        //{
        //    List<Lgda> lgdas = await UserDomainByUserId(id);
        //    foreach (var item in lgdas)
        //    {

        //    }
        //    return null;
        //}

        public async Task<List<UserLcda>> UserRoleDomainbyUserId(Guid id)
        {
            List<UserLcda> lst = await lcdaDao.UserRoleDomainbyUserId(id);

            return lst;
        }
    }
}
