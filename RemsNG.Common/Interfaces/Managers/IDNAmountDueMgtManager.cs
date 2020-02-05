using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDNAmountDueMgtManager
    {
        Task<List<DNAmountDueModel>> ByBillingNo(long billingno);
        Task<List<DNAmountDueModel>> ByBillingNo(long[] bills);
        Task<Response> UpdateAmount(DNAmountDueModel dnamount);
        void CurrentAmountDue(List<DNAmountDueModel> UnpaidDueList, decimal amountPaid, bool isFullyPaid);
        string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistoryModel dnph, string status, string createdby);
        Task<string> TogglePrepayment(long id);
        Task<PrepaymentModel[]> GetPrepayment(Guid taxpayerId);
    }
}
