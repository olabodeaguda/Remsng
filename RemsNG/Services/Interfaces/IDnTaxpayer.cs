using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDnTaxpayer
    {
        Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel);
        Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchIdAsync(string batchId);
        Task<DemandNoticeTaxpayersDetail> GetSingleTaxpayerAsync(string taxpayerId, int billingYr);

    }
}
