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
                 .Join(db.Set<DemandNoticeTaxpayer>(),
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

            var arrears = await db.Set<DemandNoticeArrear>()
                 //.Include(x => x.Item)
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayer>(),
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
                     //itemCode = s.itm.Item.ItemCode,
                     //itemDescription = s.itm.Item.ItemDescription,
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(arrears);

            var penalty = await db.Set<DemandNoticePenalty>()
                 .Include(p => p.TaxPayer)
                 .ThenInclude(s => s.Street)
                 .ThenInclude(s => s.Ward)
                 .Join(db.Set<DemandNoticeTaxpayer>(),
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
                     lastModifiedDate = s.itm.LastModifiedDate,
                     addressName = s.dnt.AddressName
                 }).ToListAsync();
            results.AddRange(penalty);

            return results;
        }

        public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        {
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            List<ItemReportSummaryModel> results = new List<ItemReportSummaryModel>();
            var items = await db.Set<DemandNoticeItem>()
                 .Include(p => p.Item)
                 .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.dn_taxpayersDetailsId, dnt => dnt.Id, (itm, dnt) => new { itm, dnt })
                 .Where(q =>
                 q.dnt.DemandNotice.Ward.WardStatus == "ACTIVE" &&
                ((q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                || (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate))
                && !q.dnt.IsUnbilled
                 &&
                status.Any(x => x == q.itm.ItemStatus)).ToListAsync();

            var r = items.Select(s => new ItemReportSummaryModel
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
            }).ToList();

            results.AddRange(r);

            var arrears = await db.Set<DemandNoticeArrear>()
                 .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && ((q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 || (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate))
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

            results.AddRange(arrears);

            var penalty = await db.Set<DemandNoticePenalty>()
                  .Join(db.Set<DemandNoticeTaxpayer>()
                 .Include(x => x.DemandNotice)
                 .ThenInclude(j => j.Street)
                 .ThenInclude(j => j.Ward),
                 itm => itm.TaxpayerId, dnt => dnt.TaxpayerId, (itm, dnt) => new { itm, dnt })
                 .Where(q => q.itm.TaxPayer.Street.Ward.WardName == "ACTIVE"
                 && ((q.itm.DateCreated >= startDate && q.itm.DateCreated <= endDate)
                 || (q.itm.LastModifiedDate >= startDate && q.itm.LastModifiedDate <= endDate))
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

            results.AddRange(penalty);
            
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
