using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeTaxpayerManager
    {
        Task<(decimal amountDue, List<AmountDueModel> amountDueDetails)> AmountDue(long billingNo);
        Task<DemandNoticeReportModel> ByBillingNo(long billingNo);
        Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchNoAsync(string batchno);
        Task<DemandNoticeTaxpayersModel> TaxpayerMiniByBillingNo(long billingNo);
        Task<Response> CancelTaxpayerDemandNoticeByBillingNo(long billingNo, string createdBy);
        Task<List<DemandNoticeTaxpayersModel>> Search(string query);
        Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId);
        Task<List<object>> GetTaxpayerPayables(Guid taxpayerId);
        Task<bool> MoveToBill(long billno);
        Task<bool> MoveToUnBills(long billno);
        Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchByLcdaId(DemandNoticeRequestModel rhModel,
            PageModel pageModel, Guid lcdaId);
        Task<bool> Delete(Guid[] ids);
    }
}
