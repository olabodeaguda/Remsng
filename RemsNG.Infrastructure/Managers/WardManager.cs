using Microsoft.EntityFrameworkCore;
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
    public class WardManager : IWardManager
    {
        private IWardRepository wardDao;
        public WardManager(IWardRepository wardRepository)
        {
            wardDao = wardRepository;
        }

        public async Task<List<WardModel>> ActiveWard()
        {
            return await wardDao.ActiveWard();
        }

        public async Task<bool> Add(WardModel ward)
        {
            return await wardDao.Add(ward);
        }

        public async Task<List<WardModel>> all()
        {
            return await wardDao.All();
        }

        public async Task<DomainModel> GetDomain(Guid wardId)
        {
            return await wardDao.GetDomain(wardId);
        }

        public async Task<WardModel> GetWard(Guid id)
        {
            return await wardDao.GetWard(id);
        }

        public async Task<WardModel> GetWard(string wardName, Guid lgdaid)
        {
            return await wardDao.GetWard(wardName, lgdaid);
        }

        public async Task<List<WardModel>> GetWardByLGDAId(Guid lgdaId)
        {
            return await wardDao.GetWardByLGDAId(lgdaId);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await wardDao.Paginated(pageModel);
        }

        public async Task<object> Paginated(PageModel pageModel, Guid lgdaId)
        {
            return await wardDao.Paginated(pageModel, lgdaId);
        }

        public async Task<bool> Update(WardModel ward)
        {
            return await wardDao.Update(ward);
        }
    }
}
