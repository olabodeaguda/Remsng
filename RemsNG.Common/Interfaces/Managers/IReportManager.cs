using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IReportManager
    {
        Task<(long[] billNumbers, Guid[] taxpayerIds)> AllIdsByDate(DateTime startDate, DateTime endDate);
        Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate);
        Task<List<ChartReportModel>> ReportByCurrentYear();
        Task<List<DemandNoticeItemModel>> ReportitemsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(DateTime startDate, DateTime endDate);
        Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate);


        Task<List<DemandNoticeItemModel>> ReportitemsByCategory(long[] billnumbers);
        Task<List<DemandNoticeArrearsModel>> ReportArrearsByCategory(Guid[] taxpayerIds);
        Task<List<DemandNoticePenaltyModel>> ReportPenaltyByCategory(Guid[] taxpayerIds);
    }
}
