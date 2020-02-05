using Microsoft.EntityFrameworkCore;
using Remsng.Data.Entities;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DNAmountDueMgtRepository : IDNAmountDueMgtRepository
    {
        private readonly DbContext db;
        public DNAmountDueMgtRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(long billingno)
        {
            DemandNoticeTaxpayer dnTaxpayer = await db.Set<DemandNoticeTaxpayer>().FirstOrDefaultAsync(x => x.BillingNumber == billingno);
            if (dnTaxpayer == null)
            {
                throw new NotFoundException("Demand notice does not exist");
            }

            string[] status = { "PART_PAYMENT", "PENDING" };

            List<DNAmountDueModel> results = new List<DNAmountDueModel>();
            var arrears = await db.Set<DemandNoticeArrear>()
                .Where(r => r.TaxpayerId == dnTaxpayer.TaxpayerId && r.BillingYear == dnTaxpayer.BillingYr && status.Any(x => x == r.ArrearsStatus))
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

            var penalty = await db.Set<DemandNoticePenalty>().Where(x => x.TaxpayerId == dnTaxpayer.TaxpayerId
            && x.BillingYear == dnTaxpayer.BillingYr && status.Any(d => d == x.ItemPenaltyStatus)).Select(x => new DNAmountDueModel
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
                 .Where(p => p.dn_taxpayersDetailsId == dnTaxpayer.Id).Select(x => new DNAmountDueModel
                 {
                     id = x.Id,
                     itemAmount = x.ItemAmount,
                     amountPaid = x.AmountPaid,
                     billingNo = x.BillingNo,
                     category = "Items",
                     itemDescription = x.Item.ItemDescription,
                     itemStatus = x.ItemStatus
                 }).ToListAsync();
            results.AddRange(items);

            //var prepayment = await db.Set<Prepayment>()
            //    .Where(x => x.taxpayerId == dnTaxpayer.TaxpayerId && x.prepaymentStatus == "ACTIVE")
            //    .Select(x => new DNAmountDueModel
            //    {
            //        itemAmount = x.amount,
            //        amountPaid = 0,
            //        billingNo = x.id,
            //        category = "Prepayment",
            //        itemDescription = string.Empty,
            //        itemStatus = x.prepaymentStatus
            //    })
            //    .ToListAsync();


            return results;
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(long[] bills)
        {
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
            DemandNoticeTaxpayer[] dnTaxpayer = await db.Set<DemandNoticeTaxpayer>().Where(x => bills.Any(p => p == x.BillingNumber)).ToArrayAsync();
            if (dnTaxpayer.Length <= 0)
            {
                throw new NotFoundException("Demand notice does not exist");
            }

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
                  .Where(p => dnTaxpayer.Any(x => x.BillingNumber == p.BillingNo && x.TaxpayerId == p.TaxpayerId) && status.Any(x => x == p.ItemStatus))
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
            if (dnamount.category.ToUpper() == "ARREARS".ToUpper())
            {
                string query = $"Update tbl_demandNoticeArrears set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";

                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category.ToUpper() == "PENALTY".ToUpper())
            {
                string query = $"Update tbl_demandNoticePenalty set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category.ToUpper() == "ITEMS".ToUpper())
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

        public async Task<string> GetQueryUpdateAmount(DNAmountDueModel[] dNAmountDueModels, DemandNoticeStatus status, string createdby)
        {
            string query = "";

            foreach (var tm in dNAmountDueModels)
            {
                if (tm.category.ToUpper() == "ARREARS".ToUpper())
                {
                    query = query + $"update tbl_demandNoticeArrears set arrearsStatus = '{status.ToString()}', " +
                               $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                }
                else if (tm.category.ToUpper() == "PENALTY".ToUpper())
                {
                    query = query + $"update tbl_demandNoticePenalty set itemPenaltyStatus = '{status}', " +
                               $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                }
                else if (tm.category.ToUpper() == "ITEMS".ToUpper())
                {
                    query = query + $"update tbl_demandNoticeItem set itemStatus = '{status}', " +
                              $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.id}';";
                }
            }
            return query;
        }

        public async Task<PrepaymentModel[]> GetPrepayment(Guid taxpayerId)
        {
            return await db.Set<Prepayment>()
                 .Where(x => x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE")
                 .Select(s => new PrepaymentModel
                 {
                     amount = s.amount,
                     datecreated = s.datecreated,
                     id = s.id,
                     prepaymentStatus = s.prepaymentStatus,
                     taxpayerId = s.taxpayerId
                 })
                 .ToArrayAsync();
        }
    }
}
