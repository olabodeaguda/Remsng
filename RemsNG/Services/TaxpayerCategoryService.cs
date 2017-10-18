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
    public class TaxpayerCategoryService : ITaxpayerCategoryService
    {
        private TaxpayerCatgoryDao taxpayerCatgoryDao;
        public TaxpayerCategoryService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            taxpayerCatgoryDao = new TaxpayerCatgoryDao(_db);
        }

        public async Task<Response> Add(TaxpayerCategory taxpayerCategory)
        {
            return await taxpayerCatgoryDao.Add(taxpayerCategory);
        }

        public async Task<Response> Delete(Guid id)
        {
            return await taxpayerCatgoryDao.Delete(id);
        }

        public async Task<TaxpayerCategory> GetById(Guid id)
        {
            return await taxpayerCatgoryDao.GetById(id);
        }

        public async Task<object> GetByNameAndLcdaId(Guid lcdaid, string name)
        {
            return await taxpayerCatgoryDao.GetByNameAndLcdaId(lcdaid, name);
        }

        public async Task<List<TaxpayerCategory>> GetListByLcdaIdAsync(Guid lcdaId)
        {
            return await taxpayerCatgoryDao.GetListByLcdaIdAsync(lcdaId);
        }

        public async Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel)
        {
            return await taxpayerCatgoryDao.GetListByLcdaIdAsync(lcdaId, pageModel);
        }

        public async Task<Response> Update(TaxpayerCategory taxpayerCategory)
        {
            return await taxpayerCatgoryDao.Update(taxpayerCategory);
        }
    }
}
