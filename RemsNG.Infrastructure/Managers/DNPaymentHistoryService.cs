using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DNPaymentHistoryManagers : IDNPaymentHistoryManagers
    {
        private DNPaymentHistoryRepository _dnpDao;
        private DemandNoticePaymentHistoryRepository dnph;
        public DNPaymentHistoryManagers(DbContext _db)
        {
            dnph = new DemandNoticePaymentHistoryRepository(_db);
            _dnpDao = new DNPaymentHistoryRepository(_db);
        }
        public async Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.AddAsync(dnphModel);
        }
        public async Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.UpdateAsync(dnphModel);
        }
        public async Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.UpdateStatusAsync(dnphModel);
        }
        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(string billingnumber)
        {
            return await dnph.ByBillingNumber(billingnumber);
        }
        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(string billingnumber)
        {
            return await dnph.ByBillingNumbers(billingnumber);
        }
        public async Task<DemandNoticePaymentHistoryModel> ById(Guid id)
        {
            return await dnph.ById(id);
        }
        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await dnph.ByLcdaId(lcdaId, pageModel);
        }
        public async Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id)
        {
            return await dnph.ByIdExtended(id);
        }
        public async Task<PrepaymentModel> GetPrepaymentByTaxpayerId(Guid taxpayerId)
        {
            return await _dnpDao.Get(taxpayerId);
        }

        public async Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment)
        {
            return await _dnpDao.AddPrepaymentForAlreadyRegisterdAmount(prepayment);
        }
    }
}
