using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticeArrearRepository
    {
        Task<List<DemandNoticeArrearsModel>> ByBillingNumber(long billingno);
        Task<List<DemandNoticeArrearsModel>> ByBillingNumber(Guid taxpayerId);
        Task<List<DemandNoticeArrearsModel>> ByTaxpayer(Guid taxpayerId);
        Task<DemandNoticeArrearsModel[]> ByTaxpayer(Guid[] taxpayerIds);
        Task<bool> AddArrears(DemandNoticeArrearsModel x);
        Task<List<DemandNoticeArrearsModel>> ReportByCategory(Guid[] taxpayerIds);
        Task<List<DemandNoticeArrearsModel>> ReportByCategory(DateTime fromDate, DateTime toDate);
        Task<bool> AddArrears(DemandNoticeArrearsModel[] models);
        Task<bool> UpdateArrearsStatus(DemandNoticeArrearsModel[] models, string status);
        Task<List<DemandNoticeArrearsModelExt>> ReportByCategoryExt(DateTime fromDate, DateTime toDate);
        Task<List<DemandNoticeArrearsModelExt>> ReportByCategoryExt(Guid[] taxpayerIds);
    }
}
