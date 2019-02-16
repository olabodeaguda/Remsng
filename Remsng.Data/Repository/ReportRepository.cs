using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
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
            return await db.Set<ChartReportModel>().FromSql("sp_reportByYear @p0",
                new object[]
                {
                    DateTime.Now.Year
                }).ToListAsync();
        }

    }
}
