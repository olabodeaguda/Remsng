using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IExcelService
    {
        Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
             string domainName, string lcdaName, DateTime startDate, DateTime enndDate);

        //Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
        //    string domainName, string lcdaName, DateTime startDate, DateTime endDate);

        Task<byte[]> WriteReportSummaryConsolidated(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistoryModel> dnph);


        Task<string> WriteReportSummaryConsolidatedSeperate(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistoryModel> dnph);
        Task<byte[]> WriteReportCategory(string domainName, string lcdaName, DateTime startDate,
            DateTime endDate, List<DemandNoticeItemModel> dnitem, List<DemandNoticePenaltyModel> dnPenalty,
            List<DemandNoticeArrearsModel> dnArrears);
        Task<byte[]> TaxpayerWithOutDemandNotice(TaxPayerModel[] taxpayers, int billingYear);
        Task<byte[]> TaxpayerReportByWard(List<ItemReportSummaryModel> rptLst
            , string domainName, string lcdaName, DateTime startDate, DateTime enndDate);
    }
}
