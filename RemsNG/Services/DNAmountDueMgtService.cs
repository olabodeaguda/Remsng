using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class DNAmountDueMgtService : IDNAmountDueMgtService
    {
        private DNAmountDueMgtDao dNAmountDueMgtDao;
        public DNAmountDueMgtService(RemsDbContext _db)
        {
            dNAmountDueMgtDao = new DNAmountDueMgtDao(_db);
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string billingno)
        {
            return await dNAmountDueMgtDao.ByBillingNo(billingno);
        }        

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            return await dNAmountDueMgtDao.UpdateAmount(dnamount);
        }

        public void CurrentAmountDue(List<DNAmountDueModel> UnpaidDueList,decimal amountPaid,bool isFullyPaid)
        {
            if (!isFullyPaid)
            {
                decimal totalAmountDue = UnpaidDueList.Sum(x => (x.itemAmount - x.amountPaid));

                foreach (var tm in UnpaidDueList)
                {
                    decimal itemShare = (tm.itemAmount - tm.amountPaid) / totalAmountDue;
                    tm.amountPaid = tm.amountPaid + decimal.Round((itemShare * amountPaid),2);
                } 
            }
            else
            {
                foreach (var tm in UnpaidDueList)
                {
                    tm.amountPaid = tm.itemAmount;
                }
            }

        }

        public string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistory dnph, string status, string createdby)
        {
            return dNAmountDueMgtDao.PaymentQuery(paymentDueList, dnph, status,createdby);
        }
    }
}
