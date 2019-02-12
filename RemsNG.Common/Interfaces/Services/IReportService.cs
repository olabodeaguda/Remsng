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
        Task<List<ChartReport>> ReportByCurrentYear();
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst);
        Task<List<DemandNoticeItemExt>> ReportitemsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeArrearsExt>> ReportArrearsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeItemPenaltyExt>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate);
    }
}
