using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDnTaxpayer
    {
        Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel);
        Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchIdAsync(string batchId);
        Task<DemandNoticeTaxpayersDetail> GetSingleTaxpayerAsync(string taxpayerId, int billingYr);
        Task<DemandNoticeTaxpayersDetail> ByBillingNo(string billingNo);

    }
}
