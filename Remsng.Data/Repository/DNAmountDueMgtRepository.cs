﻿using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DNAmountDueMgtRepository : AbstractRepository
    {
        public DNAmountDueMgtRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string billingno)
        {
            //var result = await db.Set<DNAmountDueModel>()
            //    .FromSql("sp_getBillingNumberTotalDue @p0", new object[] { billingno }).ToListAsync();

            List<DNAmountDueModel> results = new List<DNAmountDueModel>();
            var arrears = await db.Set<DemandNoticeArrear>()
                .Where(r => r.BillingNo == billingno)
                .Select(x => new DNAmountDueModel()
                {
                    id = x.Id,
                    itemAmount = x.TotalAmount,
                    amountPaid = x.AmountPaid,
                    billingNo = x.BillingNo,
                    category = "ARREARS",
                    itemDescription = "description",
                    //itemId = x.ItemId,
                    itemStatus = x.ArrearsStatus
                }).ToListAsync();
            results.AddRange(arrears);
            DemandNoticeTaxpayer dnTaxpayer = await db.Set<DemandNoticeTaxpayer>().FirstOrDefaultAsync(x => x.BillingNumber == billingno);
            if (dnTaxpayer != null)
            {
                throw new NotFoundException("Demand notice does not exist");
            }

            var penalty = await db.Set<DemandNoticePenalty>().Where(x => x.TaxpayerId == dnTaxpayer.TaxpayerId).Select(x => new DNAmountDueModel
            {
                id = x.Id,
                itemAmount = x.TotalAmount,
                amountPaid = x.AmountPaid,
                billingNo = x.BillingNo,
                category = "PENALTY",
                itemDescription = "Penalty",
                itemStatus = x.ItemPenaltyStatus
            }).ToListAsync();
            results.AddRange(penalty);
            var items = await db.Set<DemandNoticeItem>().Include(s => s.Item)
                 .Where(p => p.BillingNo == billingno).Select(x => new DNAmountDueModel
                 {
                     id = x.Id,
                     itemAmount = x.ItemAmount,
                     amountPaid = x.AmountPaid,
                     billingNo = x.BillingNo,
                     category = "PENALTY",
                     itemDescription = "Penalty",
                     itemStatus = x.ItemStatus
                 }).ToListAsync();
            results.AddRange(items);
            return results;
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string[] bills)
        {
            //string bnos = bills.FormatString();

            //string query = $"select dn.id,dn.totalAmount as itemAmount,dn.amountPaid,dn.arrearsStatus as itemStatus, " +
            //    $"'description' as itemDescription,'ARREARS' as category,dn.itemId, dn.billingNo " +
            //    $"from tbl_demandNoticeArrears as dn " +
            //    //$"inner join tbl_item as tm on tm.id = dn.itemId " +
            //    $"where dn.billingNo in ({bnos})  and dn.arrearsStatus in ('PART_PAYMENT','PENDING')" +
            //    $"union " +
            //    $"select dn.id,dn.totalAmount,dn.amountPaid,dn.itemPenaltyStatus as itemStatus, " +
            //    $"tm.itemDescription,'PENALTY' as category,dn.itemId, dn.billingNo " +
            //    $"from tbl_demandNoticePenalty as dn " +
            //    $"inner join tbl_item as tm on tm.id = dn.itemId " +
            //    $"where billingNo in ({bnos})  and dn.itemPenaltyStatus in ('PART_PAYMENT','PENDING')" +
            //    $"union " +
            //    $"select dn.id,dn.itemAmount,dn.amountPaid,dn.itemStatus, " +
            //    $"tm.itemDescription,'ITEMS' as category,dn.itemId, dn.billingNo " +
            //    $"from tbl_demandNoticeItem as dn " +
            //    $"inner join tbl_item as tm on tm.id = dn.itemId " +
            //    $"where billingNo in ({bnos})  and dn.itemStatus in ('PART_PAYMENT','PENDING')";
            //return await db.Set<DNAmountDueModel>()
            //    .FromSql(query).ToListAsync();

            string[] status = { "PART_PAYMENT", "PENDING" };

            List<DNAmountDueModel> results = new List<DNAmountDueModel>();
            var arrears = await db.Set<DemandNoticeArrear>()
                .Where(p => bills.Any(x => x == p.BillingNo) && status.Any(x => x == p.ArrearsStatus))
                .Select(x => new DNAmountDueModel()
                {
                    id = x.Id,
                    itemAmount = x.TotalAmount,
                    amountPaid = x.AmountPaid,
                    billingNo = x.BillingNo,
                    category = "ARREARS",
                    itemDescription = "description",
                    //itemId = x.ItemId,
                    itemStatus = x.ArrearsStatus
                }).ToListAsync();
            results.AddRange(arrears);
            //DemandNoticeTaxpayers dnTaxpayer = await db.Set<DemandNoticeTaxpayers>().FirstOrDefaultAsync(x => x.BillingNumber == billingno);
            //if (dnTaxpayer != null)
            //{
            //    throw new NotFoundException("Demand notice does not exist");
            //}

            var penalty = await db.Set<DemandNoticePenalty>()
                 .Where(p => bills.Any(x => x == p.BillingNo) && status.Any(x => x == p.ItemPenaltyStatus))
                .Select(x => new DNAmountDueModel
                {
                    id = x.Id,
                    itemAmount = x.TotalAmount,
                    amountPaid = x.AmountPaid,
                    billingNo = x.BillingNo,
                    category = "PENALTY",
                    itemDescription = "Penalty",
                    itemStatus = x.ItemPenaltyStatus
                }).ToListAsync();
            results.AddRange(penalty);

            var items = await db.Set<DemandNoticeItem>().Include(s => s.Item)
                  .Where(p => bills.Any(x => x == p.BillingNo) && status.Any(x => x == p.ItemStatus))
                 .Select(x => new DNAmountDueModel
                 {
                     id = x.Id,
                     itemAmount = x.ItemAmount,
                     amountPaid = x.AmountPaid,
                     billingNo = x.BillingNo,
                     category = "PENALTY",
                     itemDescription = "Penalty",
                     itemId = x.ItemId,
                     itemStatus = x.ItemStatus
                 }).ToListAsync();
            results.AddRange(items);
            return results;

        }

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            int count = 0;
            if (dnamount.category == "ARREARS")
            {
                string query = $"Update tbl_demandNoticeArrears set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";

                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "PENALTY")
            {
                string query = $"Update tbl_demandNoticePenalty set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "ITEMS")
            {
                string query = $"Update tbl_demandNoticeItem set itemAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }

            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Update was not successful.Please try again!!!"
                };
            }

        }

        public string PaymentQuery(List<DNAmountDueModel> paymentDueList,
            DemandNoticePaymentHistoryModel dnph, string status, string createdby)
        {
            string query = "";

            if (paymentDueList.Count > 0)
            {
                foreach (var tm in paymentDueList)
                {
                    switch (tm.category)
                    {
                        case "ARREARS":
                            query = query + $"update tbl_demandNoticeArrears set amountPaid = {tm.amountPaid}, arrearsStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        case "PENALTY":
                            query = query + $"update tbl_demandNoticePenalty set amountPaid = {tm.amountPaid}, itemPenaltyStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        case "ITEMS":
                            query = query + $"update tbl_demandNoticeItem set amountPaid = {tm.amountPaid}, itemStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                            break;
                        default:
                            break;
                    }
                }

                query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where billingNumber = '{dnph.BillingNumber}';";
                query = query + $"update tbl_demandNoticePaymentHistory set paymentStatus = 'APPROVED' where id = '{dnph.Id}';";
            }

            return query;
        }
    }
}
