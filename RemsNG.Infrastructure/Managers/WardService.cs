using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class WardService : IWardService
    {
        private WardRepository wardDao;
        public WardService(RemsDbContext db)
        {
            wardDao = new WardRepository(db);
        }

        public async Task<List<Ward>> ActiveWard()
        {
            return await wardDao.ActiveWard();
        }

        public async Task<bool> Add(Ward ward)
        {
            return await wardDao.Add(ward);
        }

        public async Task<List<Ward>> all()
        {
            return await wardDao.All();
        }

        public async Task<Domain> GetDomain(Guid wardId)
        {
            return await wardDao.GetDomain(wardId);
        }

        public async Task<Ward> GetWard(Guid id)
        {
            return await wardDao.GetWard(id);
        }

        public async Task<Ward> GetWard(string wardName, Guid lgdaid)
        {
            return await wardDao.GetWard(wardName, lgdaid);
        }

        public async Task<List<Ward>> GetWardByLGDAId(Guid lgdaId)
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

        public async Task<bool> Update(Ward ward)
        {
            return await wardDao.Update(ward);
        }
    }
}
