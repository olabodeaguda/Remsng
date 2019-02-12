using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeService
    {
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Response> UpdateStatus(DemandNotice demandNotice);
        Task<Response> UpdateBillingYr(DemandNotice demandNotice);
        Task<Response> UpdateQuery(DemandNotice demandNotice);
        Task<Response> Add(DemandNotice demandNotice);
        Task<DemandNotice> GetById(Guid id);
        Task<object> All(PageModel pageModel);
        Task<DemandNotice> GetByBatchId(string batchId);
        Task<bool> AddArrears(DemandNoticeArrears dna);
        Task<object> SearchDemandNotice(DemandNotice demandNotice, PageModel pageModel);

    }
}
