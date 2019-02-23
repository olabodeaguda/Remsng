using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IReportManagers
    {
        Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate);
        Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate);
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList);
        Task<List<ChartReportModel>> ReportByCurrentYear();
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst);
        Task<List<DemandNoticeItemModel>> ReportitemsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate);
    }
}
