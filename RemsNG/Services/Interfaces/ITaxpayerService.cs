using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
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
    }
}
