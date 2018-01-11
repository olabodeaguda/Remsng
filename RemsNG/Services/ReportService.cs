using RemsNG.Dao;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ReportService : IReportService
    {
        private readonly ReportDao reportDao;
        public ReportService(RemsDbContext _db)
        {
            reportDao = new ReportDao(_db);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate(startDate, endDate);
        }

        public async void ReportSummaryByDate(DateTime startDate, DateTime endDate)
        {
            List<ItemReportSummaryModel> datas = await ByDate(startDate, endDate);

        }
    }
}
