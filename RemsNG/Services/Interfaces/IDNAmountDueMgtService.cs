using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDNAmountDueMgtService
    {
        Task<List<DNAmountDueModel>> ByBillingNo(string billingno);
        Task<Response> UpdateAmount(DNAmountDueModel dnamount);
        void CurrentAmountDue(List<DNAmountDueModel> UnpaidDueList, decimal amountPaid, bool isFullyPaid);
        string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistory dnph, string status);
    }
}
