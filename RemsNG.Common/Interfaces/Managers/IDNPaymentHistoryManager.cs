using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDNPaymentHistoryManager
    {
        Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnph);
        Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnph);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(long billingnumber);
        Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(long[] billingnumber);
        Task<DemandNoticePaymentHistoryModel> ById(Guid id);
        Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<PrepaymentModel> GetPrepaymentByTaxpayerId(Guid taxpayerId);
        Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment);
        Task<bool> ApprovePayment(Guid id, DemandNoticeStatus status);
    }
}
