using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList,
             string domainName, string lcdaName, DateTime startDate, DateTime enndDate);

        Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
            string domainName, string lcdaName, DateTime startDate, DateTime endDate);

        Task<byte[]> WriteReportSummaryConsolidated(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistory> dnph);
    }
}
