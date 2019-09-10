using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticePaymentHistoryRepository
    {
        Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnph);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(long billingnumber);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(long[] billingnumber);
        Task<DemandNoticePaymentHistoryModel> ById(Guid id);
        Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<List<DemandNoticePaymentHistoryModel>> ApprovedPaymentHistory(Guid ownerId, int currentYr);

    }
}
