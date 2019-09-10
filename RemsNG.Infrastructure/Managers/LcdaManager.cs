using Microsoft.EntityFrameworkCore;
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
    public class LcdaManager : ILcdaManager
    {
        private readonly ILcdaRepository lcdaDao;
        private readonly IRoleRepository roleDao;
        private readonly ITaxpayerRepository taxpayerDao;
        private readonly IWardRepository wardDao;
        private readonly IStreetRepository streetDao;
        public LcdaManager(ILcdaRepository lcdaRepository, IRoleRepository roleRepository,
            ITaxpayerRepository taxpayerRepository, IWardRepository wardRepository,
            IStreetRepository streetRepository)
        {
            lcdaDao = lcdaRepository;
            roleDao = roleRepository;
            taxpayerDao = taxpayerRepository;
            wardDao = wardRepository;
            streetDao = streetRepository;
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
                WardModel ward = await wardDao.GetWard(dnr.wardId);
                if (ward != null)
                {
                    lgda = await lcdaDao.Get(ward.LcdaId);
                }
            }
            else if (dnr.streetId != null)
            {
                StreetModel str = await streetDao.ById(dnr.streetId);
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

        public async Task<LcdaModel> ByBillingNumber(long billingno)
        {
            return await lcdaDao.ByBillingNumber(billingno);
        }

    }
}
