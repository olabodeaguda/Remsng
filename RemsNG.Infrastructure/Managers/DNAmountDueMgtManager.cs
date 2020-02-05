using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DNAmountDueMgtManager : IDNAmountDueMgtManager
    {
        private IDNAmountDueMgtRepository dNAmountDueMgtDao;
        public DNAmountDueMgtManager(IDNAmountDueMgtRepository dNAmountDueMgtRepository)
        {
            dNAmountDueMgtDao = dNAmountDueMgtRepository;
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(long billingno)
        {
            return await dNAmountDueMgtDao.ByBillingNo(billingno);
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(long[] bills)
        {
            return await dNAmountDueMgtDao.ByBillingNo(bills);
        }

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            return await dNAmountDueMgtDao.UpdateAmount(dnamount);
        }

        public void CurrentAmountDue(List<DNAmountDueModel> UnpaidDueList,
            decimal amountPaid, bool isFullyPaid)
        {
            if (!isFullyPaid)
            {
                decimal totalAmountDue = UnpaidDueList.Sum(x => (x.itemAmount - x.amountPaid));

                foreach (var tm in UnpaidDueList)
                {
                    decimal itemShare = (tm.itemAmount - tm.amountPaid) / totalAmountDue;
                    tm.amountInitialPaid = itemShare * amountPaid;
                    tm.amountPaid = tm.amountPaid + decimal.Round((tm.amountInitialPaid), 2);
                }
                decimal totalAmount = UnpaidDueList.Sum(x => x.amountInitialPaid);
                decimal remaining = amountPaid - totalAmount;
                if (remaining > 0)
                {
                    UnpaidDueList[0].amountInitialPaid = UnpaidDueList[0].amountInitialPaid + remaining;
                    UnpaidDueList[0].amountInitialPaid = UnpaidDueList[0].amountPaid + remaining;
                }
            }
            else
            {
                foreach (var tm in UnpaidDueList)
                {
                    tm.amountPaid = tm.itemAmount;
                    tm.amountInitialPaid = tm.itemAmount;
                }
            }

        }

        public string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistoryModel dnph, string status, string createdby)
        {
            return dNAmountDueMgtDao.PaymentQuery(paymentDueList, dnph, status, createdby);
        }

        public async Task<string> TogglePrepayment(long id)
        {
            if (id == default(long))
            {
                throw new InvalidCredentialsException("Please select atleast a prepayment");
            }

            return await dNAmountDueMgtDao.TogglePrepayment(id);
        }

        public async Task<PrepaymentModel[]> GetPrepayment(Guid taxpayerId)
        {
            if (taxpayerId == default(Guid))
            {
                throw new InvalidCredentialsException("Please selet atleast a prepayment");
            }
            return await dNAmountDueMgtDao.GetPrepayment(taxpayerId);
        }
    }
}
