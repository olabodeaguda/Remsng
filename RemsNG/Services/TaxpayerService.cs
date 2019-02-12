using Microsoft.Extensions.Logging;
using RemsNG.Dao;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class TaxpayerService : ITaxpayerService
    {
        private IDNAmountDueMgtService amountDueMgtService;
        private TaxpayerRepository taxpayerDao;
        public TaxpayerService(RemsDbContext _db, ILoggerFactory loggerFactory, IDNAmountDueMgtService _amountDueMgtService)
        {
            taxpayerDao = new TaxpayerRepository(_db);
            amountDueMgtService = _amountDueMgtService;
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

        public async Task<List<TaxpayerExtension>> Search(Guid lcdaId, string qu)
        {
            return await taxpayerDao.Search(lcdaId, qu);
        }

        public async Task<Response> Update(Taxpayer taxpayer)
        {
            return await taxpayerDao.Update(taxpayer);
        }

        public async Task<List<DemandNoticePaymentHistory>> PaymentHistory(Guid id)
        {
            if (id == default(Guid))
            {
                throw new UserValidationException("Invalid request");
            }
            List<DemandNoticePaymentHistory> lst = new List<DemandNoticePaymentHistory>();
            var result = await taxpayerDao.PaymentHistory(id);
            foreach (var tm in result)
            {
                var r = await amountDueMgtService.ByBillingNo(tm.billingNumber);
                tm.TotalBillAmount = r.Sum(x => x.itemAmount);
                lst.Add(tm);
            }
            return lst;
        }

        public async Task<bool> Delete(Guid taxpayerId)
        {
            if (taxpayerId == default(Guid))
            {
                throw new UserValidationException("Invalid request");
            }
            int result = await taxpayerDao.UpdateStatus(taxpayerId, TaxpayerEnum.DELETED.ToString());
            if (result >= 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<TaxpayerExtension>> SearchInStreet(Guid streetId, string query)
        {
            return await taxpayerDao.SearchInStreet(streetId, query);
        }

        public async Task<TaxpayerExtension2[]> UnBilledTaxpayer(int billingYear)
        {
            return await taxpayerDao.GetUnbilledTaxpayer(billingYear);
        }
    }
}
