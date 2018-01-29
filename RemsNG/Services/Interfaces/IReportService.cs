using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate);
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList);
        Task<List<ChartReport>> ReportByCurrentYear();
        Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst);
    }
}
