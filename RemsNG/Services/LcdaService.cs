using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;
using Microsoft.Extensions.Logging;

namespace RemsNG.Services
{
    public class LcdaService : ILcdaService
    {
        private readonly LcdaDao lcdaDao;
        private readonly RoleDao roleDao;
        private readonly TaxpayerDao taxpayerDao;
        private readonly WardDao wardDao;
        private readonly StreetDao streetDao;
        public LcdaService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            lcdaDao = new LcdaDao(_db,loggerFactory);
            roleDao = new RoleDao(_db, loggerFactory);
            taxpayerDao = new TaxpayerDao(_db);
            wardDao = new WardDao(_db);
            streetDao = new StreetDao(_db,loggerFactory);
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

        public async Task<List<Lgda>> UserRoleDomainbyUserId(Guid id)
        {
            List<Lgda> lst = await lcdaDao.UserDomainByUserId(id);

            return lst;
        }

        public async Task<List<Lgda>> UnAssignUserDomainByUserId(Guid userid)
        {
            return await lcdaDao.unAssignUserDomainByUserId(userid);
        }

        public async Task<bool> RemoveUserFromLCDA(UserLcda userLcda)
        {
            return await lcdaDao.RemoveUserFromLCDA(userLcda);
        }

        public async Task<Lgda> ByStreet(Guid streetId)
        {
            return await lcdaDao.ByStreet(streetId);
        }

        public async Task<Domain> GetDomain(Guid lcdaId)
        {
            return await lcdaDao.GetDomain(lcdaId);
        }

        public async Task<Lgda> Get(DemandNoticeRequest dnr)
        {
            Lgda lgda = null;
            if (dnr.wardId != null)
            {
                Ward ward = await wardDao.GetWard(dnr.wardId.Value);
                if (ward != null)
                {
                    lgda = await lcdaDao.Get(ward.lcdaId);
                }
            }
            else if (dnr.streetId != null)
            {
                Street str = await streetDao.ById(dnr.streetId.Value);
                if (str != null)
                {
                    Ward ward = await wardDao.GetWard(str.wardId);
                    if (ward != null)
                    {
                        lgda = await lcdaDao.Get(ward.lcdaId);
                    }
                }
            }

            return lgda;
        }

        public async Task<Lgda> GetLcdaExtension(Guid lcdaId)
        {
            return await lcdaDao.GetLcdaExtension(lcdaId);
        }

        public async Task<Lgda> ByBillingNumber(string billingno)
        {
            return await lcdaDao.ByBillingNumber(billingno);
        }
    }
}
