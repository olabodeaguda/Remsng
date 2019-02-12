using RemsNG.Common.Models;
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
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistoryModel> dnph);


        Task<string> WriteReportSummaryConsolidatedSeperate(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistoryModel> dnph);
        Task<byte[]> WriteReportCategory(string domainName, string lcdaName, DateTime startDate,
            DateTime endDate, List<DemandNoticeItemModelExt> dnitem, List<DemandNoticeItemPenaltyModelExt> dnPenalty,
            List<DemandNoticeArrearsModelExt> dnArrears);
        Task<byte[]> TaxpayerWithOutDemandNotice(TaxpayerExtensionModel2[] taxpayers, int billingYear);
    }
}
