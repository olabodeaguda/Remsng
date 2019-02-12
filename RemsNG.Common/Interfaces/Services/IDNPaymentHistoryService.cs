using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDNPaymentHistoryService
    {
        Task<Response> AddAsync(DemandNoticePaymentHistory dnph);
        Task<Response> UpdateAsync(DemandNoticePaymentHistory dnph);
        Task<Response> UpdateStatusAsync(DemandNoticePaymentHistory dnph);
        Task<List<DemandNoticePaymentHistoryExt>> ByBillingNumber(string billingnumber);
        Task<List<DemandNoticePaymentHistory>> ByBillingNumbers(string billingnumber);
        Task<DemandNoticePaymentHistory> ById(Guid id);
        Task<DemandNoticePaymentHistory> ByIdExtended(Guid id);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Prepayment> GetPrepaymentByTaxpayerId(Guid taxpayerId);
        Task<Prepayment> AddPrepaymentForAlreadyRegisterdAmount(Prepayment prepayment);
    }
}
