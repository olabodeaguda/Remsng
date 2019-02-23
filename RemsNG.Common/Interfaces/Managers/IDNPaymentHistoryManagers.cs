using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDNPaymentHistoryManagers
    {
        Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnph);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(string billingnumber);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(string billingnumber);
        Task<DemandNoticePaymentHistoryModel> ById(Guid id);
        Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<PrepaymentModel> GetPrepaymentByTaxpayerId(Guid taxpayerId);
        Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment);
    }
}
