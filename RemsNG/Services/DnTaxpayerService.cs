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
    public class DnTaxpayerService : IDnTaxpayer
    {
        private readonly DemandNoticeTaxpayersDao dnTaxpayerDao;
        public DnTaxpayerService(RemsDbContext _db)
        {
            dnTaxpayerDao = new DemandNoticeTaxpayersDao(_db);
        }

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            return await dnTaxpayerDao.GetDNTaxpayerByBatchIdAsync(batchId, pageModel);
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchIdAsync(string batchId)
        {
            return await dnTaxpayerDao.GetDNTaxpayerByBatchIdAsync(batchId);
        }

        public async Task<DemandNoticeTaxpayersDetail> GetSingleTaxpayerAsync(string taxpayerId, int billingYr)
        {
            return await dnTaxpayerDao.GetSingleTaxpayerAsync(taxpayerId,billingYr);
        }
    }
}
