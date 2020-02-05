using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DNPaymentHistoryManager : IDNPaymentHistoryManager
    {
        private readonly IDNAmountDueMgtRepository _amountDueRposiotry;
        private readonly IAbstractManager _abstractService;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IDemandNoticeTaxpayerManager _dntMgr;
        private readonly IDemandNoticePaymentHistoryRepository dnph;
        public DNPaymentHistoryManager(IDemandNoticeTaxpayerManager dntMgr,
            IDemandNoticePaymentHistoryRepository demandNoticePaymentHistoryRepository,
            IHttpContextAccessor httpContextAccessor, IAbstractManager abstractService,
            IDNAmountDueMgtRepository dNAmountDueMgtRepository)
        {
            _amountDueRposiotry = dNAmountDueMgtRepository;
            _abstractService = abstractService;
            _httpAccessor = httpContextAccessor;
            _dntMgr = dntMgr;
            dnph = demandNoticePaymentHistoryRepository;
        }
        public async Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.AddAsync(dnphModel);
        }
        public async Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.UpdateAsync(dnphModel);
        }
        public async Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnphModel)
        {
            return await dnph.UpdateStatusAsync(dnphModel);
        }
        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(long billingnumber)
        {
            return await dnph.ByBillingNumber(billingnumber);
        }
        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(long[] billingnumber)
        {
            return await dnph.ByBillingNumbers(billingnumber);
        }
        public async Task<DemandNoticePaymentHistoryModel> ById(Guid id)
        {
            return await dnph.ById(id);
        }
        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await dnph.ByLcdaId(lcdaId, pageModel);
        }
        public async Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id)
        {
            return await dnph.ByIdExtended(id);
        }
        public async Task<PrepaymentModel> GetPrepaymentByTaxpayerId(Guid taxpayerId)
        {
            return await dnph.GetPrepayment(taxpayerId);
        }
        public async Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment)
        {
            return await dnph.AddPrepaymentForAlreadyRegisterdAmount(prepayment);
        }
        public async Task<bool> ApprovePayment(Guid id, DemandNoticeStatus status)
        {
            var payment = await dnph.ById(id);
            if (payment == null)
                throw new Exception("Payment can not be found");

            if (status == DemandNoticeStatus.CANCELED)
                return await dnph.UpdateStatus(id, status);

            decimal amount = payment.Amount;

            string query = string.Empty;
            // get amount due
            var amtDue = await _dntMgr.AmountDue(payment.BillingNumber);

            if (amount > amtDue.amountDue)
            {
                // paid, prepayment
                query = dnph.PaymentQuery(amtDue.amountDueDetails, payment, DemandNoticeStatus.PAID.ToString(), _httpAccessor.HttpContext.User.Identity.Name);
                //build prepayment
                decimal remainAmount = amount - amtDue.amountDue;
                query = query + $"insert into tbl_prepayment(taxpayerId,amount,datecreated,prepaymentStatus) values('{payment.OwnerId}','{remainAmount}',getdate(),'ACTIVE');";
            }
            else if (amount < amtDue.amountDue)
            {
                //partpayment
                query = dnph.PaymentQuery(amtDue.amountDueDetails, payment, DemandNoticeStatus.PART_PAYMENT.ToString(), _httpAccessor.HttpContext.User.Identity.Name);
            }
            else if (amount == amtDue.amountDue)
            {
                // paid
                query = dnph.PaymentQuery(amtDue.amountDueDetails, payment, DemandNoticeStatus.PAID.ToString(), _httpAccessor.HttpContext.User.Identity.Name);
            }

            bool result = await _abstractService.ExecuteQueryAsync(query);

            if (!result)
                return false;

            await UpdatePrepayment(amtDue.amountDueDetails);
            return true;
        }
        public async Task<bool> ApprovePaymentV2(Guid id, DemandNoticeStatus status)
        {
            var payment = await dnph.ById(id);
            if (payment == null)
                throw new Exception("Payment can not be found");

            if (status == DemandNoticeStatus.CANCELED)
                return await dnph.UpdateStatus(id, status);

            string query = string.Empty;

            PrepaymentModel[] prepaymentModels = await _amountDueRposiotry.GetPrepayment(payment.OwnerId);

            if (prepaymentModels.Length > 0)
            {
                foreach (var tm in prepaymentModels)
                {
                    query = query + $"update tbl_prepayment set prepaymentStatus = 'CLOSED',datecreated=getdate(), billingNo={payment.BillingNumber} where id='{tm.id}';";
                }
            }

            decimal amountPaid = payment.Amount + prepaymentModels.Sum(s => s.amount);

            // get amount due
            var amtDuelist = await _amountDueRposiotry.ByBillingNo(payment.BillingNumber);

            decimal amountDue = amtDuelist.Sum(x => x.itemAmount);
            DemandNoticeStatus paymentStatus = default(DemandNoticeStatus);
            if (amountPaid == amountDue)
            {
                paymentStatus = DemandNoticeStatus.PAID;
            }
            else if (amountPaid < amountDue)
            {
                paymentStatus = DemandNoticeStatus.PART_PAYMENT;
            }
            else if (amountPaid > amountDue)
            {
                paymentStatus = DemandNoticeStatus.OVERPAYMENT;
                decimal remainAmount = amountPaid = amountDue;
                query = query + $"insert into tbl_prepayment(taxpayerId,amount,datecreated,prepaymentStatus) values('{payment.OwnerId}','{remainAmount}',getdate(),'ACTIVE');";
            }
            string createdBy = _httpAccessor.HttpContext.User.Identity.Name;

            query = query + await _amountDueRposiotry.GetQueryUpdateAmount(amtDuelist.ToArray(), paymentStatus, createdBy);

            // update demand notice taxpayer table
            query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where billingNumber = '{payment.BillingNumber}';";
            query = query + $"update tbl_demandNoticePaymentHistory set paymentStatus = 'APPROVED',lastModifiedDate=getdate(),lastmodifiedby='{createdBy}' where id = '{payment.Id}';";

            bool result = await _abstractService.ExecuteQueryAsync(query);

            if (!result)
                return false;

            return true;
        }


        public async Task UpdatePrepayment(List<AmountDueModel> amtDue)
        {
            var re = amtDue.Where(x => x.Category == Category.Prepayment).ToArray();
            if (re.Length > 0)
                await dnph.UpdatePrepaymentStatus(re.Select(x => long.Parse(x.Id)).ToArray(), "CLOSED");
        }

        public async Task<decimal> TotalAmountPaid(long billerNumber)
        {
            return await dnph.TotalAmountPaid(billerNumber);
        }

        public async Task<(decimal closed, decimal active)> GetPrepayment(Guid taxpayerId, long billingNo)
        {
            var raw = await dnph.GetPrepayment(taxpayerId, billingNo);

            var active = raw.Where(x => x.prepaymentStatus == "ACTIVE").ToArray();
            var closed = raw.Where(x => x.prepaymentStatus == "CLOSED").ToArray();

            return (closed.Sum(x => x.amount), active.Sum(x => x.amount));
        }
    }
}
