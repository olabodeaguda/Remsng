using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;

namespace RemsNG.Dao
{
    public class ReportDao : AbstractRepository
    {
        public ReportDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await db.ItemReportSummaryModels.FromSql("sp_paymentSummaryByItems @p0,@p1",
                       new object[]
                       {
                    startDate,endDate
                       }).ToListAsync();
            }
            catch (Exception x)
            {

                throw;
            }
        }

        public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        {
            return await db.ItemReportSummaryModels.FromSql("sp_paymentSummaryByItems2 @p0,@p1",
                new object[]
                {
                    startDate,endDate
                }).ToListAsync();
        }

        public async Task<List<ChartReport>> ReportByYear()
        {
            return await db.ChartReports.FromSql("sp_reportByYear @p0",
                new object[]
                {
                    DateTime.Now.Year
                }).ToListAsync();
        }

    }
}
