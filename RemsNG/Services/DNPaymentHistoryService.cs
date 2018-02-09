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
    public class DNPaymentHistoryService : IDNPaymentHistoryService
    {
        private DemandNoticePaymentHistoryDao dnph;
        public DNPaymentHistoryService(RemsDbContext _db)
        {
            dnph = new DemandNoticePaymentHistoryDao(_db);
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

        public async Task<List<DemandNoticePaymentHistory>> ByBillingNumber(string billingnumber)
        {
            return await dnph.ByBillingNumber(billingnumber);
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
    }
}
