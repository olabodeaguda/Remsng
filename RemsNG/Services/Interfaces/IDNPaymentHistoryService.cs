using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDNPaymentHistoryService
    {
        Task<Response> AddAsync(DemandNoticePaymentHistory dnph);
        Task<Response> UpdateAsync(DemandNoticePaymentHistory dnph);
        Task<Response> UpdateStatusAsync(DemandNoticePaymentHistory dnph);
        Task<List<DemandNoticePaymentHistory>> ByBillingNumber(string billingnumber);
        Task<List<DemandNoticePaymentHistory>> ByBillingNumbers(string billingnumber);
        Task<DemandNoticePaymentHistory> ById(Guid id);
        Task<DemandNoticePaymentHistory> ByIdExtended(Guid id);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
    }
}
