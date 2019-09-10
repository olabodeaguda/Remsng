using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDNAmountDueMgtRepository
    {
        Task<List<DNAmountDueModel>> ByBillingNo(long billingno);
        Task<List<DNAmountDueModel>> ByBillingNo(long[] bills);
        Task<Response> UpdateAmount(DNAmountDueModel dnamount);
        string PaymentQuery(List<DNAmountDueModel> paymentDueList,
            DemandNoticePaymentHistoryModel dnph, string status, string createdby);
    }
}
