using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
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


        Task<string> WriteReportSummaryConsolidatedSeperate(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistory> dnph);
        Task<byte[]> WriteReportCategory(string domainName, string lcdaName, DateTime startDate,
            DateTime endDate, List<DemandNoticeItemExt> dnitem, List<DemandNoticeItemPenaltyExt> dnPenalty,
            List<DemandNoticeArrearsExt> dnArrears);
        Task<byte[]> TaxpayerWithOutDemandNotice(TaxpayerExtension2[] taxpayers, int billingYear);
    }
}
