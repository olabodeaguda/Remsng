using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ReportRepository : AbstractRepository
    {
        public ReportRepository(DbContext _db) : base(_db)
        {
        }

        //public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        //{
        //    //try
        //    //{
        //    List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();
        //    var items = await db.Set<DemandNoticeItem>()
        //         .Include(x => x.Item)
        //         .Include(p => p.TaxPayer)
        //         .ThenInclude(s => s.Street)
        //         .ThenInclude(s => s.Ward)
        //         .Join(db.Set<DemandNoticeTaxpayer>(),
        //         itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
        //         .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
        //         && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
        //         && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
        //         .Select(s => new ItemReportSummaryModel
        //         {
        //             id = s.itm.Id,
        //             itemAmount = s.itm.ItemAmount,
        //             amountPaid = s.itm.AmountPaid,
        //             billingNo = s.itm.BillingNo,
        //             category = "ITEMS",
        //             wardId = s.itm.TaxPayer.Street.WardId,
        //             wardName = s.itm.TaxPayer.Street.Ward.WardName,
        //             taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
        //             itemCode = s.itm.Item.ItemCode,
        //             itemDescription = s.itm.Item.ItemDescription,
        //             lastModifiedDate = s.itm.LastModifiedDate,
        //             addressName = s.dnt.AddressName
        //         }).ToListAsync();
        //    results.AddRange(items);

        //    var arrears = await db.Set<DemandNoticeArrear>()
        //         //.Include(x => x.Item)
        //         .Include(p => p.TaxPayer)
        //         .ThenInclude(s => s.Street)
        //         .ThenInclude(s => s.Ward)
        //         .Join(db.Set<DemandNoticeTaxpayer>(),
        //         itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
        //         .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
        //         && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
        //         && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
        //         .Select(s => new ItemReportSummaryModel
        //         {
        //             id = s.itm.Id,
        //             itemAmount = s.itm.TotalAmount,
        //             amountPaid = s.itm.AmountPaid,
        //             billingNo = s.itm.BillingNo,
        //             category = "ARREARS",
        //             wardId = s.itm.TaxPayer.Street.WardId,
        //             wardName = s.itm.TaxPayer.Street.Ward.WardName,
        //             taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
        //             //itemCode = s.itm.Item.ItemCode,
        //             //itemDescription = s.itm.Item.ItemDescription,
        //             lastModifiedDate = s.itm.LastModifiedDate,
        //             addressName = s.dnt.AddressName
        //         }).ToListAsync();
        //    results.AddRange(arrears);

        //    var penalty = await db.Set<DemandNoticePenalty>()
        //         .Include(p => p.TaxPayer)
        //         .ThenInclude(s => s.Street)
        //         .ThenInclude(s => s.Ward)
        //         .Join(db.Set<DemandNoticeTaxpayer>(),
        //         itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
        //         .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
        //         && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
        //         && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
        //         .Select(s => new ItemReportSummaryModel
        //         {
        //             id = s.itm.Id,
        //             itemAmount = s.itm.TotalAmount,
        //             amountPaid = s.itm.AmountPaid,
        //             billingNo = s.itm.BillingNo,
        //             category = "PENALTY",
        //             wardId = s.itm.TaxPayer.Street.WardId,
        //             wardName = s.itm.TaxPayer.Street.Ward.WardName,
        //             taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
        //             lastModifiedDate = s.itm.LastModifiedDate,
        //             addressName = s.dnt.AddressName
        //         }).ToListAsync();
        //    results.AddRange(penalty);

        //    return results;
        //}

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            // get pending notice created at that those date
            var dnBtwDate = await db.Set<DemandNoticeTaxpayer>()
                .Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate && status.Any(p => p == x.DemandNoticeStatus)).ToListAsync();

            // get all payment made during those period
            var payBtwDate = await db.Set<DemandNoticePaymentHistory>()
                .Where(x => x.LastModifiedDate >= startDate && x.LastModifiedDate <= endDate && x.PaymentStatus == "APPROVED")
                .ToListAsync();


            // merge the bill numbers
            long[] billsNUmber = dnBtwDate.Select(x => x.BillingNumber).ToArray()
                .Concat(payBtwDate.Select(x => x.BillingNumber).ToArray())
                .Distinct()
                .ToArray();

            // get the report
            var allDemandNotice = await db.Set<DemandNoticeTaxpayer>().Where(x => billsNUmber.Any(p => p == x.BillingNumber)).ToArrayAsync();
            var allPayment = await db.Set<DemandNoticePaymentHistory>()
                .Where(x => billsNUmber.Any(p => p == x.BillingNumber) && x.LastModifiedDate <= endDate && x.PaymentStatus == "APPROVED")
                .ToArrayAsync();

            #region Query result
            var items = await db.Set<DemandNoticeItem>()
                    .Include(p => p.Item)
                    .Join(db.Set<DemandNoticeTaxpayer>()
                    .Include(x => x.DemandNotice)
                    .ThenInclude(j => j.Street)
                    .ThenInclude(j => j.Ward),
                    itm => itm.dn_taxpayersDetailsId, dnt => dnt.Id, (itm, dnt) => new { itm, dnt })
                    .Where(q =>
                    q.dnt.DemandNotice.Ward.WardStatus == "ACTIVE" &&
                   allDemandNotice.Any(x => x.Id == q.dnt.Id)
                   && !q.dnt.IsUnbilled)
                    .Select(s => new ItemReportSummaryModel
                    {
                        id = s.itm.Id,
                        itemAmount = s.itm.ItemAmount,
                        amountPaid = s.itm.AmountPaid,
                        billingNo = s.itm.BillingNo,
                        category = "ITEMS",
                        wardId = s.dnt.DemandNotice.WardId.Value,
                        wardName = s.dnt.WardName,
                        taxpayersName = s.dnt.TaxpayersName,
                        itemCode = s.itm.Item.ItemCode,
                        itemDescription = s.itm.Item.ItemDescription,
                        lastModifiedDate = s.itm.LastModifiedDate ?? s.itm.DateCreated,
                        addressName = s.dnt.AddressName
                    }).ToListAsync();

            var arrears = await db.Set<DemandNoticeArrear>()
                 .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && allDemandNotice.Any(x => x.Id == q.dnt.Id)
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
                     //itemCode = s.itm.Item.ItemCode,
                     //itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate ?? s.itm.DateCreated,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();

            var penalty = await db.Set<DemandNoticePenalty>()
                  .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && allDemandNotice.Any(x => x.Id == q.dnt.Id)
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
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            #endregion

            var dats = items.Concat(arrears).Concat(penalty).ToList();
            List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();

            foreach (var item in dats.GroupBy(x => x.billingNo))
            {
                decimal paymentAmount = allPayment.Where(x => x.BillingNumber == item.Key).Sum(x => x.Amount);
                var data = item.ToList();
                if (paymentAmount <= 0)
                {
                    results.AddRange(data.Select(x =>
                    {
                        var r = x;
                        r.amountPaid = 0;
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
