using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticeItemRepository
    {
        Task<Response> Add(DemandNoticeItemModel[] items);
        Task<List<DemandNoticeItemModel>> ByBillingNumber(long billingno);
        Task<List<DemandNoticeItemModel>> UnpaidBillsByTaxpayerId(Guid taxpayerId,
            long billNumber, int billingYr);
        Task<List<DemandNoticeItemModel>> ReportByCategory(DateTime fromDate, DateTime toDate);
        Task<List<DemandNoticeItemModel>> ReportByCategory(long[] billNumbers);
        Task<DemandNoticeItemModelExt[]> ReportByCatgory(long[] billNumbers);
    }
}
