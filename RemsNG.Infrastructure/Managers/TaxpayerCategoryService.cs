using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class TaxpayerCategoryManagers : ITaxpayerCategoryManagers
    {
        private TaxpayerCatgoryRepository taxpayerCatgoryDao;
        public TaxpayerCategoryManagers(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            taxpayerCatgoryDao = new TaxpayerCatgoryRepository(_db);
        }

        public async Task<Response> Add(TaxpayerCategoryModel taxpayerCategory)
        {
            return await taxpayerCatgoryDao.Add(taxpayerCategory);
        }

        public async Task<Response> Delete(Guid id)
        {
            return await taxpayerCatgoryDao.Delete(id);
        }

        public async Task<TaxpayerCategoryModel> GetById(Guid id)
        {
            return await taxpayerCatgoryDao.GetById(id);
        }

        public async Task<object> GetByNameAndLcdaId(Guid lcdaid, string name)
        {
            return await taxpayerCatgoryDao.GetByNameAndLcdaId(lcdaid, name);
        }

        public async Task<List<TaxpayerCategoryModel>> GetListByLcdaIdAsync(Guid lcdaId)
        {
            return await taxpayerCatgoryDao.GetListByLcdaIdAsync(lcdaId);
        }

        public async Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel)
        {
            return await taxpayerCatgoryDao.GetListByLcdaIdAsync(lcdaId, pageModel);
        }

        public async Task<Response> Update(TaxpayerCategoryModel taxpayerCategory)
        {
            return await taxpayerCatgoryDao.Update(taxpayerCategory);
        }

        public async Task<TaxpayerCategoryModel> GetTaxpayerCategory(Guid taxpayerId)
        {
            return await taxpayerCatgoryDao.GetTaxpayerCategory(taxpayerId);
        }
    }
}
