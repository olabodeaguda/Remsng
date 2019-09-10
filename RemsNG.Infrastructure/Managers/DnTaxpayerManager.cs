using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DnTaxpayerManagers : IDnTaxpayerManager
    {
        private readonly IDemandNoticeTaxpayersRepository dnTaxpayerDao;
        public DnTaxpayerManagers(IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository)
        {
            dnTaxpayerDao = demandNoticeTaxpayersRepository;
        }

        public async Task<DemandNoticeTaxpayersModel> ByBillingNo(long billingNo)
        {
            return await dnTaxpayerDao.ByBillingNo(billingNo);
        }

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            return await dnTaxpayerDao.GetDNTaxpayerByBatchIdAsync(batchId, pageModel);
        }

        public async Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchIdAsync(string batchId)
        {
            return await dnTaxpayerDao.GetDNTaxpayerByBatchIdAsync(batchId);
        }

        public async Task<DemandNoticeTaxpayersModel> GetSingleTaxpayerAsync(string taxpayerId, int billingYr)
        {
            return await dnTaxpayerDao.GetSingleTaxpayerAsync(taxpayerId, billingYr);
        }
    }
}
