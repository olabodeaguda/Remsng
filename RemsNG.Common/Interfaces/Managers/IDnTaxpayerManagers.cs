using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDnTaxpayerManagers
    {
        Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel);
        Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchIdAsync(string batchId);
        Task<DemandNoticeTaxpayersModel> GetSingleTaxpayerAsync(string taxpayerId, int billingYr);
        Task<DemandNoticeTaxpayersModel> ByBillingNo(string billingNo);

    }
}
