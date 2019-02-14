using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Data.Repository;
using Remsng.Data;
using RemsNG.Common.Models;

namespace RemsNG.Services
{
    public class LcdaManagers : ILcdaManagers
    {
        private readonly LcdaRepository lcdaDao;
        private readonly RoleRepository roleDao;
        private readonly TaxpayerRepository taxpayerDao;
        private readonly WardRepository wardDao;
        private readonly StreetRepository streetDao;
        public LcdaManagers(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            lcdaDao = new LcdaRepository(_db,loggerFactory);
            roleDao = new RoleRepository(_db, loggerFactory);
            taxpayerDao = new TaxpayerRepository(_db);
            wardDao = new WardRepository(_db);
            streetDao = new StreetRepository(_db,loggerFactory);
        }

        public async Task<List<LcdaModel>> ActiveLcdaByDomainId(Guid domainId)
        {
            return await lcdaDao.ActiveLCDAByDomainId(domainId);
        }

        public async Task<bool> Add(LcdaModel lcda)
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

        public async Task<LcdaModel> ByLcdaCode(string lcdaCode)
        {
            return await lcdaDao.byLcdaCode(lcdaCode);
        }

        public async Task<List<LcdaModel>> byUsername(string username)
        {
            return await lcdaDao.getLcdaByUsername(username);
        }

        public async Task<bool> Changetatus(Guid id, string lcdastatus)
        {
            return await lcdaDao.Changetatus(id, lcdastatus);
        }

        public Task<LcdaModel> Get(Guid id)
        {
            return lcdaDao.Get(id);
        }

        public async Task<bool> Update(LcdaModel lcda)
        {
            return await lcdaDao.Update(lcda);
        }

        public async Task<List<LcdaModel>> UserDomainByUserId(Guid id)
        {
            return await lcdaDao.UserDomainByUserId(id);
        }

        public async Task<UserLcdaModel> UserLcdaByIds(Guid lgdaId, Guid userId)
        {
            return await lcdaDao.UserLcdaByIds(lgdaId, userId);
        }

        public async Task<List<LcdaModel>> UserRoleDomainbyUserId(Guid id)
        {
            List<LcdaModel> lst = await lcdaDao.UserDomainByUserId(id);

            return lst;
        }

        public async Task<List<LcdaModel>> UnAssignUserDomainByUserId(Guid userid)
        {
            return await lcdaDao.unAssignUserDomainByUserId(userid);
        }

        public async Task<bool> RemoveUserFromLcda(UserLcdaModel userLcda)
        {
            return await lcdaDao.RemoveUserFromLCDA(userLcda);
        }

        public async Task<LcdaModel> ByStreet(Guid streetId)
        {
            return await lcdaDao.ByStreet(streetId);
        }

        public async Task<DomainModel> GetDomain(Guid lcdaId)
        {
            return await lcdaDao.GetDomain(lcdaId);
        }

        public async Task<LcdaModel> Get(DemandNoticeRequestModel dnr)
        {
            LcdaModel lgda = null;
            if (dnr.wardId != null)
            {
                WardModel ward = await wardDao.GetWard(dnr.wardId.Value);
                if (ward != null)
                {
                    lgda = await lcdaDao.Get(ward.LcdaId);
                }
            }
            else if (dnr.streetId != null)
            {
                StreetModel str = await streetDao.ById(dnr.streetId.Value);
                if (str != null)
                {
                    WardModel ward = await wardDao.GetWard(str.WardId);
                    if (ward != null)
                    {
                        lgda = await lcdaDao.Get(ward.LcdaId);
                    }
                }
            }

            return lgda;
        }

        public async Task<LcdaModel> GetLcdaExtension(Guid lcdaId)
        {
            return await lcdaDao.GetLcdaExtension(lcdaId);
        }

        public async Task<LcdaModel> ByBillingNumber(string billingno)
        {
            return await lcdaDao.ByBillingNumber(billingno);
        }
        
    }
}
