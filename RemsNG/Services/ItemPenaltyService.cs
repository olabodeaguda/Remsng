using Microsoft.Extensions.Logging;
using RemsNG.Dao;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ItemPenaltyService : IItemPenaltyService
    {
        private ItemPenaltyDao itemPenaltyDao;
        private IDNAmountDueMgtService _admService;
        private readonly DemandNoticeTaxpayersRepository demandNoticeTaxpayersDao;
        private readonly DemandNoticePenaltyDao demandNoticePenaltyDao;
        private readonly ILogger logger;
        public ItemPenaltyService(RemsDbContext _db
            , IDNAmountDueMgtService dNAmountDueMgtService,
            ILoggerFactory loggerFactory)
        {
            itemPenaltyDao = new ItemPenaltyDao(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersRepository(_db);
            _admService = dNAmountDueMgtService;
            demandNoticePenaltyDao = new DemandNoticePenaltyDao(_db);
            logger = loggerFactory.CreateLogger("Items penalty");
        }

        public async Task<Response> Add(ItemPenalty item)
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

        public async Task<Response> Update(ItemPenalty item)
        {
            return await itemPenaltyDao.Update(item);
        }

        public async Task<Response> UpdateStatus(ItemPenalty item)
        {
            return await itemPenaltyDao.UpdateStatus(item);
        }

        public async Task<Response> RunTaxpayerPenalty(Guid[] taxpayerIds)
        {
            var recievables = await demandNoticeTaxpayersDao.GetAllReceivables(taxpayerIds);// unpaid taxpayer

            if (recievables.Length > 0)
            {
                var currentDues = await _admService.ByBillingNo(recievables.Select(x => x.billingNumber).ToArray()); // all current due amount of taxpayers
                string query = string.Empty;
                foreach (var tm in recievables)
                {
                    var res = currentDues.Where(x => x.billingNo == tm.billingNumber);
                    decimal sumitem = res.Sum(x => x.itemAmount);
                    decimal sumAmtPaid = res.Sum(x => x.amountPaid);
                    var amountRemaining = currentDues.Where(x => x.billingNo == tm.billingNumber).Sum(x => (x.itemAmount - x.amountPaid));
                    if (amountRemaining > 0)
                    {
                        DemandNoticeItemPenalty dnp = new DemandNoticeItemPenalty()
                        {
                            billingNo = tm.billingNumber,
                            amountPaid = 0,
                            billingYr = tm.billingYr,
                            itemId = Guid.Empty,
                            itemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                            originatedYear = tm.billingYr,
                            taxpayerId = tm.taxpayerId,
                            totalAmount = amountRemaining * (decimal)0.1
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

        public async Task<List<DemandNoticeItemPenalty>> ActivePenalty(Guid taxpayerId)
        {
            var result = await demandNoticePenaltyDao.ByTaxpayerId(taxpayerId);
            return result
                .Where(x => x.itemPenaltyStatus == "PENDING" || x.itemPenaltyStatus == "PART_PAYMENT")
                .ToList();
        }
    }
}
