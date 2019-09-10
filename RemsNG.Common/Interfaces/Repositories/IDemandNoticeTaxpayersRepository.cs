using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticeTaxpayersRepository
    {
        Task<List<DemandNoticeTaxpayersModel>> getTaxpayerByIds(string[] ids, int billingYr);
        Task<bool> UpdateSatus(Guid id, string status);
        Task<bool> UpdateSatus(Guid[] ids, string status);
        Task<long> NewBillingNumber();
        Task<Response> Add(DemandNoticeTaxpayersModel[] demandnotice);
        Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel);
        Task<DemandNoticeTaxpayersModel> ByBillingNo(long billingNo);
        Task<DemandNoticeTaxpayersModel> GetSingleTaxpayerAsync(string taxpayerId, int billingYr);
        Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchIdAsync(string batchId);
        Task<Response> CancelTaxpayerDemandNoticeByBillingNo(long billingNo, string createdBy);
        Task<List<DemandNoticeTaxpayersModel>> SearchAllAsync(string qs);
        Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId);
        Task<DemandNoticeTaxpayersModel[]> GetAllReceivables();
        Task<DemandNoticeTaxpayersModel[]> GetAllReceivables(Guid[] taxpayerIds);
        Task<List<DemandNoticeTaxpayersModel>> GetTaxpayerPayables(Guid taxpayerId);
        Task<bool> MoveToBills(long billno);
        Task<bool> MoveToUnBills(long billno);
        Task<PageModel<DemandNoticeTaxpayersModel[]>> Search(DemandNoticeRequestModel rhModel,
            PageModel pageModel);
        Task<DemandNoticeTaxpayersModel[]> Search(DemandNoticeRequestModel rhModel);
        Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchByLcdaId(DemandNoticeRequestModel rhModel,
            PageModel pageModel, Guid lcdaId);
        Task<DemandNoticeTaxpayersModel[]> SearchTaxpayers(DemandNoticeRequestModel rhModel);
        Task<DemandNoticeTaxpayersModel[]> ConstructByTaxpayerIds(DemandNoticeRequestModel model,
            Dictionary<string, ImagesModel> images);
        Task<DemandNoticeTaxpayersModel[]> ById(Guid[] ids);
        Task<DemandNoticeTaxpayersModel> ById(Guid id);
        Task<bool> UpdateArrearsStatus(DemandNoticeTaxpayersModel[] dntaxpayers, bool isRunArrears);
        Task<bool> UpdatePenaltyStatus(DemandNoticeTaxpayersModel[] dntaxpayers, bool isRunPenalty);
        Task<bool> UpdateAddress(Guid taxpayerId, string address);
        Task<bool> UpdateTaxpayerName(Guid taxpayerId, string name);
        Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchNoAsync(string batchno);
    }
}
