using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ITaxpayerManagers
    {
        Task<Response> Create(TaxPayerModel taxpayer, bool confirmCompany);
        Task<TaxPayerModel> ById(Guid id);
        Task<List<TaxPayerModel>> ByStreetId(Guid streetId);
        Task<object> ByStreetId(Guid streetId, PageModel pageModel);
        Task<List<TaxPayerModel>> ByCompanyId(Guid companyId);
        Task<Response> Update(TaxPayerModel taxpayer);
        Task<List<TaxPayerModel>> ByLcdaId(Guid lcdaId);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<LcdaModel> getLcda(Guid taxpayerId);
        Task<List<TaxPayerModel>> Search(Guid lcdaId, string qu);
        Task<List<DemandNoticePaymentHistoryModel>> PaymentHistory(Guid id);
        Task<bool> Delete(Guid taxpayerId);
        Task<List<TaxPayerModel>> SearchInStreet(Guid streetId, string query);
        Task<TaxPayerModel[]> UnBilledTaxpayer(int billingYear);
    }
}
