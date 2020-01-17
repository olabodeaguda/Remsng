using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ITaxpayerRepository
    {
        Task<Response> Create(TaxPayerModel taxpayer, bool confirmCompany);
        Task<TaxPayerModel> Get(Guid streetId, Guid companyId);
        Task<List<TaxPayerModel>> GetActiveTaxpayers(DemandNoticeRequestModel demandNoticeRequest);
        Task<TaxPayerModel> ById(Guid id);
        Task<List<TaxPayerModel>> ByStreetId(Guid streetId);
        Task<object> ByStreetId(Guid streetId, PageModel pageModel);
        Task<List<TaxPayerModel>> ByCompanyId(Guid companyId);
        Task<Response> Update(TaxPayerModel taxpayer);
        Task<List<TaxPayerModel>> ByLcdaId(Guid lcdaId);
        Task<List<TaxPayerModel>> Search(Guid lcdaId, string qu);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<LcdaModel> getLcda(Guid taxpayerId);
        Task<List<DemandNoticePaymentHistoryModel>> PaymentHistory(Guid taxpayerId);
        Task<int> UpdateStatus(Guid id, string status);
        Task<List<TaxPayerModel>> SearchInStreet(Guid streetid, string queryParams);
        Task<TaxPayerModel[]> GetUnbilledTaxpayer(int billingYear);
        Task<TaxPayerModel[]> SearchByDNRequest(DemandNoticeRequestModel rhModel, Guid[] excludedId);
        Task<bool> UpdateStreet(Guid taxpayerId, Guid streetId);
        Task<TaxPayerModel[]> ByTaxpayerId(Guid[] taxpayerId);
    }
}
