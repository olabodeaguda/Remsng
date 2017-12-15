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
    public class TaxpayerService : ITaxpayerService
    {
        private TaxpayerDao taxpayerDao;
        public TaxpayerService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            taxpayerDao = new TaxpayerDao(_db);
        }

        public async Task<List<TaxpayerExtension>> ByCompanyId(Guid companyId)
        {
            return await taxpayerDao.ByCompanyId(companyId);
        }

        public async Task<TaxpayerExtension> ById(Guid id)
        {
            return await taxpayerDao.ById(id);
        }

        public async Task<List<TaxpayerExtension>> ByLcdaId(Guid lcdaId)
        {
            return await taxpayerDao.ByLcdaId(lcdaId);
        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await taxpayerDao.ByLcdaId(lcdaId, pageModel);
        }

        public async Task<List<TaxpayerExtension>> ByStreetId(Guid streetId)
        {
            return await taxpayerDao.ByStreetId(streetId);
        }

        public async Task<object> ByStreetId(Guid streetId, PageModel pageModel)
        {
            return await taxpayerDao.ByStreetId(streetId, pageModel);
        }

        public async Task<Response> Create(Taxpayer taxpayer, bool confirmCompany)
        {
            return await taxpayerDao.Create(taxpayer, confirmCompany);
        }

        public async Task<Lgda> getLcda(Guid taxpayerId)
        {
            return await taxpayerDao.getLcda(taxpayerId);
        }

        public async Task<Response> Update(Taxpayer taxpayer)
        {
            return await taxpayerDao.Update(taxpayer);
        }
    }
}
