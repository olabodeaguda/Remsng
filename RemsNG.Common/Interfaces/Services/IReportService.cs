using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate);
        Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate);
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList);
        Task<List<ChartReportModel>> ReportByCurrentYear();
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst);
        Task<List<DemandNoticeItemModelExt>> ReportitemsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeArrearsModelExt>> ReportArrearsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeItemPenaltyModelExt>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate);
    }
}
