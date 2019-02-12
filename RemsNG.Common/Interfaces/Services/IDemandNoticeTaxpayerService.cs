using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeTaxpayerService
    {
        Task<DemandNoticeReportModel> ByBillingNo(string billingNo);
        Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchNoAsync(string batchno);
        Task<DemandNoticeTaxpayersModel> TaxpayerMiniByBillingNo(string billingNo);
        Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo, string createdBy);
        Task<List<DemandNoticeTaxpayersModel>> Search(string query);
        Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId);
        Task<DemandNoticeTaxpayersModel[]> GetAllReceivables();
        Task<List<object>> GetTaxpayerPayables(Guid taxpayerId);
        Task<bool> MoveToBill(string billno);
        Task<bool> MoveToUnBills(string billno);
        Task<List<DemandNoticeArrearsModel>> GetArrearsByTaxpayerId(Guid taxpayerId);
    }
}
