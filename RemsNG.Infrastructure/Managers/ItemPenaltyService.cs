using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ItemPenaltyManagers : IItemPenaltyManagers
    {
        private ItemPenaltyRepository itemPenaltyDao;
        private IDNAmountDueMgtManagers _admService;
        private readonly DemandNoticeTaxpayersRepository demandNoticeTaxpayersDao;
        private readonly DemandNoticePenaltyRepository demandNoticePenaltyDao;
        private readonly ILogger logger;
        public ItemPenaltyManagers(RemsDbContext _db
            , IDNAmountDueMgtManagers dNAmountDueMgtService,
            ILoggerFactory loggerFactory)
        {
            itemPenaltyDao = new ItemPenaltyManagers(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersRepository(_db);
            _admService = dNAmountDueMgtService;
            demandNoticePenaltyDao = new DemandNoticePenaltyRepository(_db);
            logger = loggerFactory.CreateLogger("Items penalty");
        }

        public async Task<Response> Add(ItemPenaltyModel item)
        {
            return await itemPenaltyDao.Add(item);
        }

        public async Task<object> GetById(Guid id)
        {
            return await itemPenaltyDao.GetById(id);
        }

        public async Task<object> ListByItemId(Guid itemId)
        {
            return await itemPenaltyDao.ListByItemId(itemId);
        }

        public async Task<object> ListByItemId(Guid itemId, PageModel pageModel)
        {
            return await itemPenaltyDao.ListByItemId(itemId, pageModel);
        }

        public async Task<Response> Update(ItemPenaltyModel item)
        {
            return await itemPenaltyDao.Update(item);
        }

        public async Task<Response> UpdateStatus(ItemPenaltyModel item)
        {
            return await itemPenaltyDao.UpdateStatus(item);
        }

        public async Task<Response> RunTaxpayerPenalty(Guid[] taxpayerIds)
        {
            var recievables = await demandNoticeTaxpayersDao.GetAllReceivables(taxpayerIds);// unpaid taxpayer

            if (recievables.Length > 0)
            {
                var currentDues = await _admService.ByBillingNo(recievables.Select(x => x.BillingNumber).ToArray()); // all current due amount of taxpayers
                string query = string.Empty;
                foreach (var tm in recievables)
                {
                    var res = currentDues.Where(x => x.billingNo == tm.BillingNumber);
                    decimal sumitem = res.Sum(x => x.itemAmount);
                    decimal sumAmtPaid = res.Sum(x => x.amountPaid);
                    var amountRemaining = currentDues.Where(x => x.billingNo == tm.BillingNumber).Sum(x => (x.itemAmount - x.amountPaid));
                    if (amountRemaining > 0)
                    {
                        DemandNoticePenaltyModel dnp = new DemandNoticePenaltyModel()
                        {
                            BillingNo = tm.BillingNumber,
                            AmountPaid = 0,
                            BillingYear = tm.BillingYr,
                            ItemId = Guid.Empty,
                            ItemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                            OriginatedYear = tm.BillingYr,
                            TaxpayerId = tm.TaxpayerId,
                            TotalAmount = amountRemaining * (decimal)0.1
                        };
                        query = query + demandNoticePenaltyDao.AddQuery(dnp);
                    }
                }
                if (!string.IsNullOrEmpty(query))
                {
                    try
                    {
                        return demandNoticePenaltyDao.RunQuery(query);
                    }
                    catch (Exception x)
                    {
                        logger.LogError(x.Message);
                        throw new UserValidationException(x.Message);
                    }
                }
            }
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "No penalty for the selected user"
            };
        }

        public async Task<List<DemandNoticeItemPenaltyModelExt>> ActivePenalty(Guid taxpayerId)
        {
            var result = await demandNoticePenaltyDao.ByTaxpayerId(taxpayerId);
            return result
                .Where(x => x.itemPenaltyStatus == "PENDING" || x.itemPenaltyStatus == "PART_PAYMENT")
                .ToList();
        }
    }
}
