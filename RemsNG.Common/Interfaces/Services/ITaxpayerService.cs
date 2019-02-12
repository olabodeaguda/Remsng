using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ITaxpayerService
    {
        Task<Response> Create(Taxpayer taxpayer, bool confirmCompany);
        Task<TaxpayerExtension> ById(Guid id);
        Task<List<TaxpayerExtension>> ByStreetId(Guid streetId);
        Task<object> ByStreetId(Guid streetId, PageModel pageModel);
        Task<List<TaxpayerExtension>> ByCompanyId(Guid companyId);
        Task<Response> Update(Taxpayer taxpayer);
        Task<List<TaxpayerExtension>> ByLcdaId(Guid lcdaId);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<Lgda> getLcda(Guid taxpayerId);
        Task<List<TaxpayerExtension>> Search(Guid lcdaId, string qu);
        Task<List<DemandNoticePaymentHistory>> PaymentHistory(Guid id);
        Task<bool> Delete(Guid taxpayerId);
        Task<List<TaxpayerExtension>> SearchInStreet(Guid streetId, string query);
        Task<TaxpayerExtension2[]> UnBilledTaxpayer(int billingYear);
    }
}
