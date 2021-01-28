using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeManager
    {
        Task<bool> AddDemanNotice2(DemandNoticeRequestModel model);
        Task<TaxPayerModel[]> ValidTaxpayers(DemandNoticeRequestModel model);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Response> Delete(Guid Id);
        Task<Response> UpdateBillingYr(DemandNoticeModel demandNotice);
        Task<Response> UpdateQuery(DemandNoticeModel demandNotice);
        Task<Response> Add(DemandNoticeModel demandNotice);
        Task<DemandNoticeModel> GetById(Guid id);
        Task<object> All(PageModel pageModel);
        Task<DemandNoticeModel> GetByBatchId(string batchId);
        Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchDemandNotice(DemandNoticeRequestModel rhModel, PageModel pageModel);
        Task<DemandNoticeRequestModel> SearchInfo(DemandNoticeRequestModel model);
        Task<TaxPayerModel[]> ValidTaxpayers(DemandNoticeRequestModel model, bool persist);
        Task<bool> AddDemanNotice(DemandNoticeRequestModel model);
        Task<DemandNoticeTaxpayersModel[]> SearchDemandNotice(DemandNoticeRequestModel rhModel);
        Task<object> SearchDemandNotice(DemandNoticeModel rhModel, PageModel pageModel);
    }
}
