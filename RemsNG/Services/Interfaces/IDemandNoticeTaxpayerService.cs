using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDemandNoticeTaxpayerService
    {
        Task<DemandNoticeReportModel> ByBillingNo(string billingNo);
        Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchNoAsync(string batchno);
        Task<DemandNoticeTaxpayersDetail> TaxpayerMiniByBillingNo(string billingNo);
        Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo, string createdBy);
        Task<List<DemandNoticeTaxpayersDetail>> Search(string query);
        Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId);
        Task<DemandNoticeTaxpayersDetail[]> GetAllReceivables();
        Task<List<object>> GetTaxpayerPayables(Guid taxpayerId);
    }
}
