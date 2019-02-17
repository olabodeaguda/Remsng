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
            try
            {
                return await db.Set<ItemReportSummaryModel>().
                    FromSql("sp_paymentSummaryByItems @p0,@p1",
                       new object[]
                       {
                    startDate,endDate
                       }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        {
            return await db.Set<ItemReportSummaryModel>()
                .FromSql("sp_paymentSummaryByItems2 @p0,@p1",
                new object[]
                {
                    startDate,endDate
                }).ToListAsync();
        }

        public async Task<List<ChartReportModel>> ReportByYear()
        {
            int billingYr = DateTime.Now.Year;
            var items = await db.Set<DemandNoticeItem>()
                .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
                dnt => dnt.billingNumber, (dni, dnt) => new ChartReportModel
                {
                    amountPaid = dni.AmountPaid,
                    itemAmount = dni.ItemAmount,
                    wardName = dnt.WardName,
                    id = dni.Id,
                    BillingYear = dnt.BillingYr
                }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var arrears = await db.Set<DemandNoticeArrears>()
               .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
               dnt => dnt.billingNumber, (dni, dnt) => new ChartReportModel
               {
                   amountPaid = dni.AmountPaid,
                   itemAmount = dni.TotalAmount,
                   wardName = dnt.WardName,
                   id = dni.Id,
                   BillingYear = dnt.BillingYr
               }).Where(x => x.BillingYear == billingYr).ToListAsync();

            var penalty = await db.Set<DemandNoticePenalty>()
               .Join(db.Set<DemandNoticeTaxpayers>(), dni => dni.BillingNo,
               dnt => dnt.billingNumber, (dni, dnt) => new ChartReportModel
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
