using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ReportManager : IReportManager
    {
        private readonly DemandNoticePenaltyRepository dnPenaltyDao;
        private readonly DemandNoticeArrearRepository dnArrearsDao;
        private readonly DemandNoticeItemRepository dnitemDao;
        private readonly ReportRepository reportDao;
        public ReportManager(DbContext _db)
        {
            reportDao = new ReportRepository(_db);
            dnitemDao = new DemandNoticeItemRepository(_db);
            dnArrearsDao = new DemandNoticeArrearRepository(_db);
            dnPenaltyDao = new DemandNoticePenaltyRepository(_db);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate(startDate, endDate);
        }

        public async Task<List<ChartReportModel>> ReportByCurrentYear()
        {
            return await reportDao.ReportByYear();
        }

        public async Task<List<DemandNoticeItemModel>> ReportitemsByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnitemDao.ReportByCategory(startDate, endDate);
        }

        public async Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnArrearsDao.ReportByCategory(startDate, endDate);
        }

        public async Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnPenaltyDao.ReportByCategory(startDate, endDate);
        }
    }
}
