using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly DbContext db;
        public ReportRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<(long[] billNumbers, Guid[] taxpayerIds)> AllIdsByDate(DateTime startDate, DateTime endDate)
        {
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            // get pending notice created at that those date

            string qry = $"select * from tbl_demandNoticeTaxpayers " +
                $"where DemandNoticeStatus in ('PAID', 'PENDING', 'PART_PAYMENT')  " +
                $"and (DateCreated >= CONVERT(varchar,'{startDate.ToString("yyyy-MM-dd")} 12:00:00',120) and DateCreated <= CONVERT(varchar,'{endDate.ToString("yyyy-MM-dd")} 11:59:00',120))";

            var dnBtwDate = await db.Set<DemandNoticeTaxpayer>()
                .FromSql(qry).ToArrayAsync();

            // get all payment made during those period
            var payBtwDate = await db.Set<DemandNoticePaymentHistory>()
                .Where(x => x.LastModifiedDate >= startDate && x.LastModifiedDate <= endDate && x.PaymentStatus == "APPROVED")
                .ToListAsync();

            // merge the bill numbers
            long[] billNUmbers = dnBtwDate.Select(x => x.BillingNumber).ToArray()
                .Concat(payBtwDate.Select(x => x.BillingNumber).ToArray())
                .Distinct()
                .ToArray();

            Guid[] txIds = dnBtwDate.Select(x => x.TaxpayerId).ToArray()
                .Concat(payBtwDate.Select(x => x.OwnerId).ToArray())
                .Distinct()
                .ToArray();

            return (billNUmbers, txIds);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            var identites = await AllIdsByDate(startDate, endDate);
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            long[] billNUmbers = identites.billNumbers;
            Guid[] txIds = identites.taxpayerIds;

            string nNums = string.Join(',', billNUmbers);

            var allPayment = await db.Set<DemandNoticePaymentHistory>()
                .Include(d => d.Bank)
                .Where(x => billNUmbers.Any(p => p == x.BillingNumber)
                && x.LastModifiedDate <= endDate && x.PaymentStatus == "APPROVED")
                .Select(e => new DemandNoticePaymentHistoryModel
                {
                    Amount = e.Amount,
                    BankId = e.BankId,
                    BankName = e.Bank.BankName,
                    BillingNumber = e.BillingNumber,
                    CreatedBy = e.CreatedBy,
                    DateCreated = e.DateCreated,
                    Id = e.Id,
                    Lastmodifiedby = e.Lastmodifiedby,
                    LastModifiedDate = e.LastModifiedDate,
                    OtherNames = e.OtherNames,
                    PaymentMode = e.PaymentMode,
                    OwnerId = e.OwnerId,
                    PaymentStatus = e.PaymentStatus,
                    ReferenceNumber = e.ReferenceNumber
                })
                .ToArrayAsync();

            #region Query result

            var items = await db.Set<DemandNoticeItem>()
                .Include(x => x.DemandNoticeTaxpayer)
                .ThenInclude(d => d.DemandNotice)
                .Where(s => s.DemandNoticeTaxpayer.DemandNotice.Ward.WardStatus == "ACTIVE"
                && billNUmbers.Contains(s.BillingNo))
                .Select(s => new ItemReportSummaryModel
                {
                    id = s.Id,
                    itemAmount = s.ItemAmount,
                    amountPaid = s.AmountPaid,
                    billingNo = s.BillingNo,
                    category = "ITEMS",
                    wardId = s.DemandNoticeTaxpayer.DemandNotice.WardId.Value,
                    wardName = s.DemandNoticeTaxpayer.WardName,
                    taxpayersName = s.DemandNoticeTaxpayer.TaxpayersName,
                    itemCode = s.Item.ItemCode,
                    itemDescription = s.Item.ItemDescription,
                    lastModifiedDate = s.LastModifiedDate ?? s.DateCreated,
                    addressName = s.DemandNoticeTaxpayer.AddressName,
                    BillingDate = s.DateCreated.Value
                })
                .ToListAsync();

            var arrears = await db.Set<DemandNoticeArrear>()
                 .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                    && billNUmbers.Any(x => x == q.dnt.BillingNumber)
                 && q.dnt.IsUnbilled == false && status.Any(x => x == q.itm.ArrearsStatus))
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "ARREARS",
                     wardId = s.dnt.DemandNotice.WardId.Value,
                     wardName = s.dnt.WardName,
                     taxpayersName = s.dnt.TaxpayersName,
                     lastModifiedDate = s.itm.LastModifiedDate ?? s.itm.DateCreated,
                     addressName = s.dnt.AddressName,
                     BillingDate = s.itm.DateCreated.Value
                 }).ToListAsync();

            var penalty = await db.Set<DemandNoticePenalty>()
                  .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                    && txIds.Contains(q.itm.TaxpayerId)
                 && q.dnt.IsUnbilled == false && status.Any(x => x == q.itm.ItemPenaltyStatus))
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "PENALTY",
                     wardId = s.dnt.DemandNotice.WardId.Value,
                     wardName = s.dnt.WardName,
                     taxpayersName = s.dnt.TaxpayersName,
                     lastModifiedDate = s.itm.LastModifiedDate ?? s.itm.DateCreated,
                     addressName = s.dnt.AddressName,
                     BillingDate = s.itm.DateCreated.Value
                 }).ToListAsync();
            #endregion

            var dats = items.Concat(arrears).Concat(penalty).ToList();
            List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();

            foreach (var item in dats.GroupBy(x => x.billingNo))
            {
                var re = allPayment.Where(x => x.BillingNumber == item.Key);
                decimal paymentAmount = re.Sum(x => x.Amount);
                var data = item.ToList();
                if (paymentAmount <= 0)
                {
                    results.AddRange(data.Select(x =>
                    {
                        var r = x;
                        r.amountPaid = 0;
                        r.BankName = "Nil";

                        return r;
                    }));
                }
                else
                {
                    decimal totalAmount = data.Sum(x => x.itemAmount);
                    List<ItemReportSummaryModel> lst = new List<ItemReportSummaryModel>();

                    foreach (var tm in data)
                    {
                        tm.amountPaid = (tm.itemAmount / totalAmount) * paymentAmount;
                        tm.amountPaid = Math.Round(tm.amountPaid, 2);
                        tm.BankName = re.FirstOrDefault().BankName;
                        tm.PaymentDate = re.FirstOrDefault().LastModifiedDate;
                        tm.Reference = re.FirstOrDefault().ReferenceNumber;
                        lst.Add(tm);
                    }
                    decimal amt = lst.Sum(s => s.amountPaid);

                    if (amt < totalAmount)
                    {
                        var u = lst.FirstOrDefault(x => x.amountPaid < x.itemAmount);
                        lst.Remove(u);
                        u.amountPaid = u.amountPaid + (totalAmount - amt);
                        lst.Add(u);
                    }

                    results.AddRange(lst);
                }
            }


            return results;
        }

        public async Task<List<ChartReportModel>> ReportByYear()
        {
            int billingYr = DateTime.Now.Year;
            var items = await db.Set<DemandNoticeItem>()
                .Join(db.Set<DemandNoticeTaxpayer>(), dni => dni.BillingNo,
                dnt => dnt.BillingNumber, (dni, dnt) => new ChartReportModel
                {
                    amountPaid = dni.AmountPaid,
                    itemAmount = dni.ItemAmount,
                    wardName = dnt.WardName,
                    id = dni.Id,
                    BillingYear = dnt.BillingYr
                }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var arrears = await db.Set<DemandNoticeArrear>()
               .Join(db.Set<DemandNoticeTaxpayer>(), dni => dni.BillingNo,
               dnt => dnt.BillingNumber, (dni, dnt) => new ChartReportModel
               {
                   amountPaid = dni.AmountPaid,
                   itemAmount = dni.TotalAmount,
                   wardName = dnt.WardName,
                   id = dni.Id,
                   BillingYear = dnt.BillingYr
               }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var penalty = await db.Set<DemandNoticePenalty>()
               .Join(db.Set<DemandNoticeTaxpayer>(), dni => dni.BillingNo,
               dnt => dnt.BillingNumber, (dni, dnt) => new ChartReportModel
               {
                   amountPaid = dni.AmountPaid,
                   itemAmount = dni.TotalAmount,
                   wardName = dnt.WardName,
                   id = dni.Id,
                   BillingYear = dnt.BillingYr
               }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var result = new List<ChartReportModel>();
            result.AddRange(items);
            result.AddRange(arrears);
            result.AddRange(penalty);

            return result;
            //await db.Set<ChartReportModel>().FromSql("sp_reportByYear @p0",
            //new object[]
            //{
            //    DateTime.Now.Year
            //}).ToListAsync();
        }

    }
}
