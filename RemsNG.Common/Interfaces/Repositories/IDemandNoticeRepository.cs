using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticeRepository
    {
        Task<Response> Add(DemandNoticeModel demandNotice);
        Task<object> SearchDemandNotice(DemandNoticeModel query, PageModel pageModel);
        Task<Response> UpdateQuery(DemandNoticeModel demandNotice);
        Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice);
        Task<Response> UpdateStatus(DemandNoticeModel demandNotice);
        Task<PageModel<List<DemandNoticeModel>>> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<object> All(PageModel pageModel);
        Task<DemandNoticeModel> GetById(Guid id);
        Task<DemandNoticeModel> GetByBatchId(string batchId);
        Task<DemandNoticeModel> GetLastEntry();
    }
}
