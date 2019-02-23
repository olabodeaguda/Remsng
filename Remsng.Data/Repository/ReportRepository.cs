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

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            //try
            //{
            List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();
            var items = await db.Set<DemandNoticeItem>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.ItemAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "ITEMS",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(items);

            var arrears = await db.Set<DemandNoticeArrears>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "ARREARS",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(arrears);

            var penalty = await db.Set<DemandNoticePenalty>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate) && q.dnt.IsUnbilled == false)
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "PENALTY",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(penalty);


            //return await db.Set<ItemReportSummaryModel>().
            //    FromSql("sp_paymentSummaryByItems @p0,@p1",
            //       new object[]
            //       {
            //    startDate,endDate
            //       }).ToListAsync();

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            return results;
        }

        public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        {
            //return await db.Set<ItemReportSummaryModel>()
            //    .FromSql("sp_paymentSummaryByItems2 @p0,@p1",
            //    new object[]
            //    {
            //        startDate,endDate
            //    }).ToListAsync();
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();
            var items = await db.Set<DemandNoticeItem>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate)
                 && q.dnt.IsUnbilled == false && status.Any(x => x == q.itm.ItemStatus))
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.ItemAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "ITEMS",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(items);

            var arrears = await db.Set<DemandNoticeArrears>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate)
                 && q.dnt.IsUnbilled == false && status.Any(x => x == q.itm.ArrearsStatus))
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "ARREARS",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(arrears);

            var penalty = await db.Set<DemandNoticePenalty>()
                 .Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 itm => itm.BillingNo, dnt => dnt.BillingNumber, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && (q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 && (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate)
                 && q.dnt.IsUnbilled == false && status.Any(x => x == q.itm.ItemPenaltyStatus))
                 .Select(s => new ItemReportSummaryModel
                 {
                     id = s.itm.Id,
                     itemAmount = s.itm.TotalAmount,
                     amountPaid = s.itm.AmountPaid,
                     billingNo = s.itm.BillingNo,
                     category = "PENALTY",
                     wardId = s.itm.TaxPayer.Street.WardId,
                     wardName = s.itm.TaxPayer.Street.Ward.WardName,
                     taxpayersName = $"{s.itm.TaxPayer.Firstname} {s.itm.TaxPayer.Lastname} {s.itm.TaxPayer.Surname}",
                     itemCode = s.itm.Item.ItemCode,
                     itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(penalty);


            //return await db.Set<ItemReportSummaryModel>().
            //    FromSql("sp_paymentSummaryByItems @p0,@p1",
            //       new object[]
            //       {
            //    startDate,endDate
            //       }).ToListAsync();

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            return results;

        }

        public async Task<List<ChartReportModel>> ReportByYear()
        {
            int billingYr = DateTime.Now.Year;
            var items = await db.Set<DemandNoticeItem>()
                .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
                dnt => dnt.BillingNumber, (dni, dnt) => new ChartReportModel
                {
                    amountPaid = dni.AmountPaid,
                    itemAmount = dni.ItemAmount,
                    wardName = dnt.WardName,
                    id = dni.Id,
                    BillingYear = dnt.BillingYr
                }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var arrears = await db.Set<DemandNoticeArrears>()
               .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
               dnt => dnt.BillingNumber, (dni, dnt) => new ChartReportModel
               {
                   amountPaid = dni.AmountPaid,
                   itemAmount = dni.TotalAmount,
                   wardName = dnt.WardName,
                   id = dni.Id,
                   BillingYear = dnt.BillingYr
               }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var penalty = await db.Set<DemandNoticePenalty>()
               .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
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
