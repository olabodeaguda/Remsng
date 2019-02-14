using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeManagers
    {
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Response> UpdateStatus(DemandNoticeModel demandNotice);
        Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice);
        Task<Response> UpdateQuery(DemandNoticeModel demandNotice);
        Task<Response> Add(DemandNoticeModel demandNotice);
        Task<DemandNoticeModel> GetById(Guid id);
        Task<object> All(PageModel pageModel);
        Task<DemandNoticeModel> GetByBatchId(string batchId);
        Task<bool> AddArrears(DemandNoticeArrearsModel dna);
        Task<object> SearchDemandNotice(DemandNoticeModel demandNotice, 
            PageModel pageModel);
    }
}
