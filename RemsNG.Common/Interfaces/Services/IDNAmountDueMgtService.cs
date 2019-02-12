﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDNAmountDueMgtService
    {
        Task<List<DNAmountDueModel>> ByBillingNo(string billingno);
        Task<List<DNAmountDueModel>> ByBillingNo(string[] bills);
        Task<Response> UpdateAmount(DNAmountDueModel dnamount);
        void CurrentAmountDue(List<DNAmountDueModel> UnpaidDueList, decimal amountPaid, bool isFullyPaid);
        string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistory dnph, string status, string createdby);
    }
}
