using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class TaxpayerManager : ITaxpayerManager
    {
        private IDemandNoticeTaxpayersRepository _dnRepo;
        private IDNAmountDueMgtManager amountDueMgtService;
        private ITaxpayerRepository taxpayerDao;
        public TaxpayerManager(IDemandNoticeTaxpayersRepository demandNoticeTaxpayersRepository,
            ILoggerFactory loggerFactory,
            IDNAmountDueMgtManager _amountDueMgtService, ITaxpayerRepository taxpayerRepository)
        {
            _dnRepo = demandNoticeTaxpayersRepository;
            taxpayerDao = taxpayerRepository;
            amountDueMgtService = _amountDueMgtService;
        }

        public async Task<List<TaxPayerModel>> ByCompanyId(Guid companyId)
        {
            return await taxpayerDao.ByCompanyId(companyId);
        }

        public async Task<TaxPayerModel> ById(Guid id)
        {
            return await taxpayerDao.ById(id);
        }

        public async Task<List<TaxPayerModel>> ByLcdaId(Guid lcdaId)
        {
            return await taxpayerDao.ByLcdaId(lcdaId);
        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await taxpayerDao.ByLcdaId(lcdaId, pageModel);
        }

        public async Task<List<TaxPayerModel>> ByStreetId(Guid streetId)
        {
            return await taxpayerDao.ByStreetId(streetId);
        }

        public async Task<object> ByStreetId(Guid streetId, PageModel pageModel)
        {
            return await taxpayerDao.ByStreetId(streetId, pageModel);
        }

        public async Task<Response> Create(TaxPayerModel taxpayer, bool confirmCompany)
        {
            return await taxpayerDao.Create(taxpayer, confirmCompany);
        }

        public async Task<LcdaModel> getLcda(Guid taxpayerId)
        {
            return await taxpayerDao.getLcda(taxpayerId);
        }

        public async Task<List<TaxPayerModel>> Search(Guid lcdaId, string qu)
        {
            return await taxpayerDao.Search(lcdaId, qu);
        }

        public async Task<Response> Update(TaxPayerModel taxpayer)
        {
            await _dnRepo.UpdateTaxpayerName(taxpayer.Id, $"{taxpayer.Surname} {taxpayer.Lastname} {taxpayer.Firstname}");
            return await taxpayerDao.Update(taxpayer);
        }

        public async Task<List<DemandNoticePaymentHistoryModel>> PaymentHistory(Guid id)
        {
            if (id == default(Guid))
            {
                throw new UserValidationException("Invalid request");
            }
            List<DemandNoticePaymentHistoryModel> lst = new List<DemandNoticePaymentHistoryModel>();
            var result = await taxpayerDao.PaymentHistory(id);
            foreach (var tm in result)
            {
                var r = await amountDueMgtService.ByBillingNo(tm.BillingNumber);
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
            int result = await taxpayerDao.UpdateStatus(taxpayerId, TaxPayerEnum.DELETED.ToString());
            if (result >= 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<TaxPayerModel>> SearchInStreet(Guid streetId, string query)
        {
            return await taxpayerDao.SearchInStreet(streetId, query);
        }

        public async Task<TaxPayerModel[]> UnBilledTaxpayer(int billingYear)
        {
            return await taxpayerDao.GetUnbilledTaxpayer(billingYear);
        }
    }
}
