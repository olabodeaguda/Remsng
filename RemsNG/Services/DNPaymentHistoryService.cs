using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class DNPaymentHistoryService : IDNPaymentHistoryService
    {
        private DNPaymentHistoryDao _dnpDao;
        private DemandNoticePaymentHistoryDao dnph;
        public DNPaymentHistoryService(RemsDbContext _db)
        {
            dnph = new DemandNoticePaymentHistoryDao(_db);
            _dnpDao = new DNPaymentHistoryDao(_db);
        }
        public async Task<Response> AddAsync(DemandNoticePaymentHistory dnphModel)
        {
            return await dnph.AddAsync(dnphModel);
        }
        public async Task<Response> UpdateAsync(DemandNoticePaymentHistory dnphModel)
        {
            return await dnph.UpdateAsync(dnphModel);
        }
        public async Task<Response> UpdateStatusAsync(DemandNoticePaymentHistory dnphModel)
        {
            return await dnph.UpdateStatusAsync(dnphModel);
        }
        public async Task<List<DemandNoticePaymentHistoryExt>> ByBillingNumber(string billingnumber)
        {
            return await dnph.ByBillingNumber(billingnumber);
        }
        public async Task<List<DemandNoticePaymentHistory>> ByBillingNumbers(string billingnumber)
        {
            return await dnph.ByBillingNumbers(billingnumber);
        }
        public async Task<DemandNoticePaymentHistory> ById(Guid id)
        {
            return await dnph.ById(id);
        }
        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await dnph.ByLcdaId(lcdaId, pageModel);
        }
        public async Task<DemandNoticePaymentHistory> ByIdExtended(Guid id)
        {
            return await dnph.ByIdExtended(id);
        }
        public async Task<Prepayment> GetPrepaymentByTaxpayerId(Guid taxpayerId)
        {
            return await _dnpDao.Get(taxpayerId);
        }

        public async Task<Prepayment> AddPrepaymentForAlreadyRegisterdAmount(Prepayment prepayment)
        {
            return await _dnpDao.AddPrepaymentForAlreadyRegisterdAmount(prepayment);
        }
    }
}
